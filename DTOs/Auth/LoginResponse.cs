namespace MyAzureDemo.DTOs.Auth;

public sealed record LoginResponse(string Token, string TokenType, int ExpiresIn);
