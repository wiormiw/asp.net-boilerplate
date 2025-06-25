using System.Security.Claims;

namespace Application.Common.Interfaces.Users;

public interface ITokenGeneratorService
{
    string GenerateToken(string userId, string email, HashSet<string> roles);
    string GenerateRefreshToken(string userId,  string email, HashSet<string> roles);
    int GetTokenExpiryInSeconds();
    int GetRefreshTokenExpiryInSeconds();
    ClaimsPrincipal? ValidateToken(string token);
}