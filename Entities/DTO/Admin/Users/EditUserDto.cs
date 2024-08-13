namespace BlazorADAuth.Entities.DTO.Admin.Users;

public class EditUserDto
{
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string DisplayName { get; set; } = default!;

    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool LockoutEnabled { get; set; }
}
