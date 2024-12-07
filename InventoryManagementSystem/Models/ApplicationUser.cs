using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSystem.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
}