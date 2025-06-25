using System.Security.Claims;
using Application.Common.Interfaces.Users;

namespace Api.Services;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public string? Id => _httpContextAccessor.HttpContext?.User.FindFirstValue("userId");
    public string? Email => _httpContextAccessor.HttpContext?.User.FindFirstValue("email");
    public HashSet<string> Roles =>
        _httpContextAccessor.HttpContext?.User
            .FindAll("role")
            .Select(x => x.Value)
            .ToHashSet()
        ?? [];

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
}