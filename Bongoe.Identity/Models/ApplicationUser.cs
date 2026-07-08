using Microsoft.AspNetCore.Identity;

namespace Bongoe.Identity.Models;

/// <summary>
/// Application user extending IdentityUser with custom properties.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the user's display name.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the date the user was created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets whether the user account is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}