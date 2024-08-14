using BlazorADAuth.Components.Account;
using BlazorADAuth.Contracts;
using BlazorADAuth.Entities.Auth;
using BlazorADAuth.Entities.DTO.Admin.Users;
using BlazorADAuth.Helpers;
using BlazorADAuth.Services.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorADAuth.Components.Pages.Admin.Users;

public partial class Create : ComponentBase
{
    [Inject] public RoleManager<ApplicationRole>? RoleManager { get; set; }
    [Inject] public AppUserManager<ApplicationUser>? UserManager { get; set; }
    [Inject] public IUserStore<ApplicationUser>? UserStore { get; set; }
    [Inject] public IAdUserService? AdUserService { get; set; }
    [Inject] public ILogger<Create>? Logger { get; set; }
    [Inject] internal IdentityRedirectManager? RedirectManager { get; set; }

    private IEnumerable<IdentityError>? identityErrors;

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    [SupplyParameterFromForm]
    private CreateUserDto Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    private List<ApplicationRole> Roles { get; set; } = [];

    private string? Message => identityErrors is null ? null : $"Error: {string.Join(", ", identityErrors.Select(error => error.Description))}";

    protected override async Task OnInitializedAsync()
    {
        Roles = await RoleManager!.Roles.OrderBy(o => o.Name).ToListAsync();
    }

    public async Task CreateNewUser(EditContext editContext)
    {
        var user = CreateAppUser();

        await UserStore!.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
        var emailStore = GetEmailStore();
        await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

        var adUser = await AdUserService!.GetAdUser(Input.UserName);

        if (adUser is null ||
            adUser.SamAccountName is null)
        {
            identityErrors = [IdentityErrorExtensions.ADInvalidUserName()];
            return;
        }

        user.DisplayName = adUser.DisplayName ?? adUser.SamAccountName;

        var result = await UserManager!.CreateAsync(user);

        if (!result.Succeeded)
        {
            identityErrors = result.Errors;
            return;
        }

        Logger!.LogInformation("Added user: `{userName}` with email: `{email}` to app.", Input.UserName, Input.Email);

        var roleResult = await UserManager.AddToRoleAsync(user, Input.Role);

        if (!roleResult.Succeeded)
        {
            identityErrors = roleResult.Errors;
            return;
        }

        Logger!.LogInformation("Added user: `{userName}` with email: `{email}` to role: `{role}`.", Input.UserName, Input.Email, Input.Role);

        RedirectManager!.RedirectToWithStatus("/admin/users", "User added!", HttpContext!);
    }

    private ApplicationUser CreateAppUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
        }
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!UserManager!.SupportsUserEmail)
        {
            throw new NotSupportedException("This UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)UserStore!;
    }
}
