using Application.Common.Contracts.Api;

namespace Application.Common.Interfaces.Users;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string email, string password, string firstName, string lastName);
    Task<Result> DeleteUserAsync(string userId);
    Task<Result> AddToRoleAsync(string userId, string role);
}