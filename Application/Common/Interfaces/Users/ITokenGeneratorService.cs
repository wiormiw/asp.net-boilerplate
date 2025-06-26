using System.Security.Claims;

namespace Application.Common.Interfaces.Users;

public interface ITokenGeneratorService
{
    string GenerateAccessToken(string userId, string email, HashSet<string> roles);
    string GenerateRefreshToken(string userId);
    ClaimsPrincipal? ValidateRefreshToken(string token);
}