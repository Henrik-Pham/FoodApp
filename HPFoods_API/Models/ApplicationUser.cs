using Microsoft.AspNetCore.Identity;

namespace HPFoods_API.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
}