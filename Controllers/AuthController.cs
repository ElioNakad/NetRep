using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAzureDemo.DTOs.Auth;
using MyAzureDemo.Services;

namespace MyAzureDemo.Controllers;

[ApiController]
[Route("")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("signup")]
    public async Task<IActionResult> Signup(SignupRequest request)
    {
        var result = await authService.SignupAsync(request);

        return result.Succeeded
            ? Ok(result.Value)
            : BadRequest(result.Errors);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await authService.LoginAsync(request);

        return result.Succeeded
            ? Ok(result.Value)
            : Unauthorized();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("addrole")]
    public async Task<IActionResult> AddRole(AddRoleRequest request)
    {
        var result = await authService.AddRoleAsync(request);

        if (result.Succeeded)
        {
            return Ok(result.Value);
        }

        return result.StatusCode == StatusCodes.Status404NotFound
            ? NotFound(result.Errors)
            : BadRequest(result.Errors);
    }
}
