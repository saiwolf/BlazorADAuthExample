using Microsoft.AspNetCore.Identity;

namespace BlazorADAuth.Entities.Auth;

public class ApplicationRole : IdentityRole
{
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime? Updated { get; set; }
}
