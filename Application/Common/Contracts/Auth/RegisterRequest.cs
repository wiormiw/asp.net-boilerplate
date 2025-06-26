namespace Application.Common.Contracts.Auth;

public record RegisterRequest(
    string UserName,
    string Email,
    string Password,
    string FirstName,
    string LastName);