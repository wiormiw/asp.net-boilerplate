using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName  { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}