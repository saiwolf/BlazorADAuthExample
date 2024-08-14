using BlazorADAuth.Entities.Auth;

namespace BlazorADAuth.Entities.DTO.Admin.Roles;

public class RoleDetailsDto
{
    public string? RoleId { get; set; } = default!;
    public string FriendlyId { get; set; } = default!;
    public string RoleName { get; set; } = default!;

    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }

    public List<ApplicationUser> Users { get; set; } = [];
}
