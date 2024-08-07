using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BlazorADAuth.Entities.Auth;

public class ApplicationUser : IdentityUser
{
    [PersonalData]
    [Required]
    public string? DisplayName { get; set; } = "";

    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime? Updated { get; set; }

    [NotMapped]
    public override string? PasswordHash { get; set; }
}
