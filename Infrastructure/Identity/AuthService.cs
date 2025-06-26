using Application.Common.Contracts.Auth;
using Application.Common.Interfaces.Users;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Application.Common.Contracts.Api;
using Domain.Constants;

namespace Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly IUser _user;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenGeneratorService _tokenGeneratorService;

    public AuthService(IUser user, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenGeneratorService tokenGeneratorService)
    {
        _user = user;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenGeneratorService = tokenGeneratorService;
    }
    
    public async Task<(Result Result, AuthResponse Data)> RegisterAsync(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return (Result.Failure(result.Errors.Select(e => e.Description)), default!);

        await _userManager.AddToRoleAsync(user, Roles.User);

        // Send email verification right after registration (WIP)

        var authDto = await GenerateAuthResultAsync(user);
        return (Result.Success(), new AuthResponse(authDto.AccessToken, authDto.ExpiresIn, authDto.RefreshToken));
    }
    
    public async Task<(Result Result, AuthResponse Data)> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return (Result.Failure(["Invalid credentials."]), default!);

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (!result.Succeeded)
            return (Result.Failure(["Invalid credentials."]), default!);

        var authDto = await GenerateAuthResultAsync(user);
        return (Result.Success(), new AuthResponse(authDto.AccessToken, authDto.ExpiresIn, authDto.RefreshToken));
    }
    
    public async Task<(Result Result, AuthResponse Data)> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var principal = _tokenGeneratorService.ValidateRefreshToken(request.RefreshToken);
        if (principal == null)
            return (Result.Failure(["Invalid refresh token."]), default!);

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null)
            return (Result.Failure(["User not found."]), default!);

        var authDto = await GenerateAuthResultAsync(user);
        return (Result.Success(), new AuthResponse(authDto.AccessToken, authDto.ExpiresIn, authDto.RefreshToken));
    }
    
    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var userId = _user.Id;
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Failure(["Unauthorized."]);

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure(["User not found."]);

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }
    
    private async Task<AuthResponse> GenerateAuthResultAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenGeneratorService.GenerateAccessToken(user.Id, user.Email!, roles.ToHashSet());
        var refreshToken = _tokenGeneratorService.GenerateRefreshToken(user.Id);

        return await Task.FromResult(new AuthResponse(
            AccessToken: accessToken,
            ExpiresIn: JwtSettings.JwtAccessExpiryInSeconds,
            RefreshToken: refreshToken
        ));
    }
}