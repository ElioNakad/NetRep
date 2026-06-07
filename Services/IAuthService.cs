using MyAzureDemo.DTOs.Auth;

namespace MyAzureDemo.Services;

public interface IAuthService
{
    Task<AuthResult<SignupResponse>> SignupAsync(SignupRequest request);

    Task<AuthResult<LoginResponse>> LoginAsync(LoginRequest request);

    Task<AuthResult<AddRoleResponse>> AddRoleAsync(AddRoleRequest request);
}
