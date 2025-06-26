namespace Application.Common.Contracts.Auth;

public record AuthResponse(
    string AccessToken,
    int ExpiresIn,
    string RefreshToken);