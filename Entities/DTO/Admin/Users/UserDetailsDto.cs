namespace BlazorADAuth.Entities.DTO.Admin.Users;

public class UserDetailsDto
{
    public string Id { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string UserPrincipalName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string DisplayName { get; set; } = default!;

    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }

    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool LockoutEnabled { get; set; }

    public List<string>? Roles { get; set; } = null;
}
