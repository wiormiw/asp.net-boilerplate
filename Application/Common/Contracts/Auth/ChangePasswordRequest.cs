namespace Application.Common.Contracts.Auth;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);