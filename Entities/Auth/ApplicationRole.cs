using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BlazorADAuth.Entities.Auth;

public class ApplicationRole : IdentityRole
{
    [Required]
    public int FriendlyId { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime? Updated { get; set; }
}
