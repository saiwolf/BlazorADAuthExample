using BlazorADAuth.Contracts;
using BlazorADAuth.Entities.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BlazorADAuth.Services.Auth;

public class AppSignInManager<TUser>(
        UserManager<TUser> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<TUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<TUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<TUser> confirmation,
        IAdUserService adUserService)
        : SignInManager<TUser>(userManager, contextAccessor,
            claimsFactory, optionsAccessor, logger, schemes, confirmation)
        where TUser : class
{
    private const string FILENAME = "~/Services/Auth/AppSignInManager.cs";

    private readonly ILogger<SignInManager<TUser>> _logger = logger;
    private readonly IAdUserService _adUserService = adUserService;

    public override async Task<SignInResult> PasswordSignInAsync(string username, string password,
        bool isPersistent, bool lockoutOnFailure)
    {
        try
        {
            var userCandidate = await UserManager.FindByNameAsync(username)
                ?? throw new Exception($"Unable to find user by username: `{username}`");
            var result = await CheckPasswordSignInAsync(userCandidate, password, true);
            if (result.Succeeded)
            {
                var adUser = await _adUserService.GetAdUser(username)
                    ?? throw new Exception($"Error looking up AD info for username: `{username}`");
                ApplicationUser appUser = await UserManager.FindByNameAsync(username) as ApplicationUser
                    ?? throw new Exception($"Error looking up DB info for username: `{username}`");
                List<Claim> claims = [
                    new Claim(ClaimTypes.Upn, adUser.UserPrincipalName!),
                    new Claim(ClaimTypes.Sid, appUser.Id.ToString()),
                    new Claim(ClaimTypes.GivenName, adUser.DisplayName!)
                ];
                await SignInWithClaimsAsync(userCandidate, isPersistent, claims);

                return SignInResult.Success;
            }
            else
            {
                throw new Exception($"Failed to sign in user: `{username}`");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[SIGN IN MANAGER]: {exMessage}", ex.Message);
            return SignInResult.Failed;
        }
    }
}
