namespace Domain.Constants;

public static class JwtSettings
{
    public const int JwtAccessExpiryInSeconds = 3600; // 1 hour
    public const int JwtRefreshExpiryInSeconds = 172800; // 2 days
}