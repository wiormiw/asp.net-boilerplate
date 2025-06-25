namespace Application.Common.Contracts.Auth;

public record AuthResponse(
    string AccessToken,
    string ExpiresIn,
    string RefreshToken);