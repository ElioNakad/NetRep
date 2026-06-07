using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyAzureDemo.DTOs.Auth;
using MyAzureDemo.Models;
using MyAzureDemo.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyAzureDemo.Services;

public sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<JwtSettings> jwtOptions) : IAuthService
{
    public async Task<AuthResult<SignupResponse>> SignupAsync(SignupRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);

        return result.Succeeded
            ? AuthResult<SignupResponse>.Success(new SignupResponse(user.Id, user.Email))
            : AuthResult<SignupResponse>.Failure(ToErrors(result));
    }

    public async Task<AuthResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return AuthResult<LoginResponse>.Failure([], StatusCodes.Status401Unauthorized);
        }

        var roles = await userManager.GetRolesAsync(user);
        var token = CreateJwtToken(user, roles);
        var expiration = TimeSpan.FromMinutes(jwtOptions.Value.ExpirationMinutes);

        return AuthResult<LoginResponse>.Success(new LoginResponse(
            token,
            "Bearer",
            (int)expiration.TotalSeconds));
    }

    public async Task<AuthResult<AddRoleResponse>> AddRoleAsync(AddRoleRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return AuthResult<AddRoleResponse>.Failure(
                [new { Message = "User not found." }],
                StatusCodes.Status404NotFound);
        }

        if (!await roleManager.RoleExistsAsync(request.Role))
        {
            var createRoleResult = await roleManager.CreateAsync(new IdentityRole(request.Role));

            if (!createRoleResult.Succeeded)
            {
                return AuthResult<AddRoleResponse>.Failure(ToErrors(createRoleResult));
            }
        }

        var addRoleResult = await userManager.AddToRoleAsync(user, request.Role);

        return addRoleResult.Succeeded
            ? AuthResult<AddRoleResponse>.Success(new AddRoleResponse(user.Email, request.Role))
            : AuthResult<AddRoleResponse>.Failure(ToErrors(addRoleResult));
    }

    private string CreateJwtToken(ApplicationUser user, IEnumerable<string> roles)
    {
        var settings = jwtOptions.Value;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(settings.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static IEnumerable<AuthErrorResponse> ToErrors(IdentityResult result)
    {
        return result.Errors.Select(error => new AuthErrorResponse(error.Code, error.Description));
    }
}
