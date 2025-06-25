namespace Application.Common.Contracts.Auth;

public record RegisterRequest(
    string Username,
    string Email,
    string Password,
    string FirstName,
    string LastName);