using BlazorADAuth.Contracts;
using BlazorADAuth.Entities.Auth;
using BlazorADAuth.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BlazorADAuth.Services.Auth;

public class AppUserManager<TUser>(
    IUserStore<TUser> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<TUser> passwordHasher,
    IEnumerable<IUserValidator<TUser>> userValidators,
    IEnumerable<IPasswordValidator<TUser>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider services,
    ILogger<UserManager<TUser>> logger,
    IAdUserService adUserService)
    : UserManager<TUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    where TUser : class
{
    private const string FILENAME = "~/Services/Auth/AppUserManager.cs";

    private readonly ILogger<UserManager<TUser>> _logger = logger;
    private readonly IAdUserService _adUserService = adUserService;

    /// <inheritdoc/>
    public override async Task<bool> CheckPasswordAsync(TUser user, string password)
    {
        try
        {
            ThrowIfDisposed();
            ArgumentNullException.ThrowIfNullOrEmpty(password, nameof(password));
            var dbUser = user as ApplicationUser ?? throw new Exception("User cannot be null.");
            AdUser? adUser = await _adUserService.AuthenticateUserAsync(dbUser.UserName!, password)
                ?? throw new Exception($"No AD user found for {dbUser.UserName}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{fileName} - [{methodName}]: {exMessage}", FILENAME, nameof(CheckPasswordAsync), ex.Message);
            return false;
        }
    }

    ///<inheritdoc/>
    public override async Task<TUser?> GetUserAsync(ClaimsPrincipal user)
    {        
        try
        {
            AdUser? adUser = await _adUserService.GetAdUser(user.Identity!)
                ?? throw new Exception($"No AD user found for {user.Identity!.Name}");
#pragma warning disable CA1416 // Validate platform compatibility
            if (adUser.SamAccountName is null)
                return null;

            return await FindByNameAsync(adUser.SamAccountName) ?? null;
#pragma warning restore CA1416 // Validate platform compatibility
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{fileName} - [{methodName}]: {exMessage}", FILENAME, nameof(GetUserAsync), ex.Message);
            return null;
        }
    }

    public async Task<TUser?> GetUserAsync(int friendlyId)
    {
        try
        {
            List<ApplicationUser>? users = await Users.ToListAsync() as List<ApplicationUser>
                ?? throw new Exception("No users found in DB!");

            ApplicationUser? user = users.FirstOrDefault(f => f.FriendlyId == friendlyId)
                ?? throw new Exception($"Unable to find user by ID: `{friendlyId}`.");

            AdUser? adUser = await _adUserService.GetAdUser(user.UserName!)
                ?? throw new Exception($"No AD user found for {user.UserName}");
#pragma warning disable CA1416 // Validate platform compatibility
            if (adUser.SamAccountName is null)
                return null;
#pragma warning restore CA1416 // Validate platform compatibility
            
            return user as TUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{fileName} - [{methodName}]: {exMessage}", FILENAME, nameof(GetUserAsync), ex.Message);
            return null;
        }
    }
}
