using BlazorADAuth.Contracts;
using BlazorADAuth.Entities.Auth;
using Microsoft.AspNetCore.Identity;

namespace BlazorADAuth.Components.Account;

internal sealed class IdentityUserAccessor(UserManager<ApplicationUser> userManager, IdentityRedirectManager redirectManager, IAdUserService adUserService)
{
    public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus("/account/invalid-user", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }

        var adUser = await adUserService.GetAdUser(context.User.Identity!);
        if (adUser is null)
        {
            redirectManager.RedirectToWithStatus("/account/invalid-user", $"Error: Unable to load AD user.", context);
        }

        user.AdUser = adUser;

        return user;
    }
}
