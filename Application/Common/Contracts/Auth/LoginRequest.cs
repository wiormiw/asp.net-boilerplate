namespace Application.Common.Contracts.Auth;

public record LoginRequest(
    string Email,
    string Password);