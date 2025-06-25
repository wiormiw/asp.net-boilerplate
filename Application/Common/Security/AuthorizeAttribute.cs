namespace Application.Common.Security;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class AuthorizeAttribute : Attribute
{
    public string Roles { get; set; } = string.Empty;
    
    // Uncomment this if you want policy based authorization.
    // public string Policies { get; set; } = string.Empty;
}