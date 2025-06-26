using Application.Common.Contracts.Api;
using Application.Common.Interfaces.Users;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;

    public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        return user?.UserName;
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string email, string password, string firstName, string lastName)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };
        
        var result = await _userManager.CreateAsync(user, password);
        
        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        return user != null ? await DeleteUserAsync(user) : Result.Success(); 
    }
    
    // Wrapper to prevent ApplicationUser leaks to Application layer.
    private async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);
        
        return result.ToApplicationResult();
    }

    public async Task<Result> AddToRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user is null)
            return Result.Failure(["User not found."]);
        
        if (!await _roleManager.RoleExistsAsync(role))
            return Result.Failure(["Role not found."]);
        
        var result = await _userManager.AddToRoleAsync(user, role);

        return result.ToApplicationResult();
    }
}