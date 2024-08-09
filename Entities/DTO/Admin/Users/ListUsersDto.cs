namespace BlazorADAuth.Entities.DTO.Admin.Users;

public class ListUserDto
{
    public string? FriendlyId { get; set; } = default!;
    public string? UserName { get; set; } = default!;
    public string? Email { get; set; } = default!;
    public List<string>? Roles { get; set; } = [];
}
