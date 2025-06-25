using System.Security.Claims;
using Application.Common.Interfaces.Users;

namespace Api.Services;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue("usedId");
    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue("email");
    public HashSet<string>? Roles => _httpContextAccessor.HttpContext?.User?
                                         .FindAll("role")
                                         .Select(x => x.Value)
                                         .ToHashSet() 
                                     ?? new HashSet<string>();

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
}