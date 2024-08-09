using System.ComponentModel.DataAnnotations;

namespace BlazorADAuth.Entities.DTO.Admin.Users;

public class CreateUserDto
{
    [Required]
    [Display(Name = "AD Username")]
    public string UserName { get; set; } = "";
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";

    [Required]
    [Display(Name = "Role")]
    public string Role { get; set; } = "User";
}
