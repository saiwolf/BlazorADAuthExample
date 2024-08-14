using BlazorADAuth.Contracts;
using BlazorADAuth.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorADAuth.Services.Auth;

public class AppRoleManager<TRole>(IRoleStore<TRole> store,
        IEnumerable<IRoleValidator<TRole>> roleValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        ILogger<RoleManager<TRole>> logger)
    : RoleManager<TRole>(store, roleValidators, keyNormalizer, errors, logger)
    where TRole : class
{
    private const string FILENAME = "~/Services/Auth/AppRoleManager.cs";

    private readonly ILogger<RoleManager<TRole>> _logger = logger;

    public async Task<TRole?> FindByIdAsync(int friendlyId)
    {
        try
        {
            List<ApplicationRole> roles = await Roles.AsNoTracking().ToListAsync() as List<ApplicationRole>
                ?? throw new Exception("No roles found in DB!");

            ApplicationRole? role = roles.FirstOrDefault(f => f.FriendlyId == friendlyId)
                ?? throw new Exception($"Unable to find role by ID: `{friendlyId}`.");

            return role as TRole;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{fileName} - [{methodName}]: {exMessage}", FILENAME, nameof(FindByIdAsync), ex.Message);
            return null;
        }
    }
}
