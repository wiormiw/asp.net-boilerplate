using Application.Common.Contracts.Api;
using Application.Common.Contracts.Auth;

namespace Application.Common.Interfaces.Users;

public interface IAuthService
{
    Task<(Result Result, AuthResponse Data)> RegisterAsync(RegisterRequest request);
    Task<(Result Result, AuthResponse Data)> LoginAsync(LoginRequest request);
    Task<(Result Result, AuthResponse Data)> RefreshTokenAsync(RefreshTokenRequest request);
    Task<Result> ChangePasswordAsync(ChangePasswordRequest request);
    Task<Result> VerifyEmail(string userId, string token);
    Task<Result> SendPasswordResetEmailAsync(ResetPasswordRequest request);
    Task<(Result Result, ResetPasswordResponse Data)> OneClickPasswordResetAsync(OneClickPasswordResetRequest request); 
}