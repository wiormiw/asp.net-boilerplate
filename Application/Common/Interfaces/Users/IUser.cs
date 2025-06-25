namespace Application.Common.Interfaces.Users;

public interface IUser
{
    public string? Id { get; }
    public string? Email { get; }
    public HashSet<string> Roles { get; }
}