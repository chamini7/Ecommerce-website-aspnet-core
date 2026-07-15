using Microsoft.AspNetCore.Identity;

namespace ECommerceAPI.Models;

// Extends the built-in Identity user with extra fields.
// This demonstrates ASP.NET Core Identity + custom user data.
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
