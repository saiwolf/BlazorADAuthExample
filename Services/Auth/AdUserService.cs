using BlazorADAuth.Contracts;
using BlazorADAuth.Entities.Auth;
using BlazorADAuth.Helpers;
using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using System.Security.Principal;

namespace BlazorADAuth.Services.Auth;

[SupportedOSPlatform("windows")]
public class AdUserService(ILogger<AdUserService> logger)
    : IAdUserService
{
    private const string FILENAME = "~/Services/Auth/AdUserService.cs";

    private readonly ILogger<AdUserService> _logger = logger;

    public Task<AdUser> GetAdUser(IIdentity identity)
    {
        try
        {
            PrincipalContext principalContext = new(ContextType.Domain);
            if (identity is null || string.IsNullOrEmpty(identity.Name))
                return Task.FromResult(new AdUser());
            var ident = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, identity.Name);
            return Task.FromResult(AdUser.CastToAdUser(ident));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{fileName} - [{methodName}]: {exMessage}", FILENAME, nameof(GetAdUser), ex.Message);
            throw;
        }
    }

    public Task<AdUser> GetAdUser(string samAccountName)
    {
        try
        {
            ArgumentNullException.ThrowIfNullOrEmpty(samAccountName, nameof(samAccountName));

            PrincipalContext principalContext = new(ContextType.Domain);            
            var ident = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, samAccountName);
            return Task.FromResult(AdUser.CastToAdUser(ident));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{fileName} - [{methodName}]: {exMessage}", FILENAME, nameof(GetAdUser), ex.Message);
            throw;
        }
    }

    public Task<AdUser> GetAdUser(Guid guid)
    {
        try
        {
            PrincipalContext principalContext = new(ContextType.Domain);
            var ident = UserPrincipal.FindByIdentity(principalContext, IdentityType.Guid, guid.ToString());
            return Task.FromResult(AdUser.CastToAdUser(ident));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{fileName} - [{methodName}]: {exMessage}", FILENAME, nameof(GetAdUser), ex.Message);
            throw;
        }
    }

    public Task<List<AdUser>> GetDomainUsers()
    {
        PrincipalContext principalContext = new(ContextType.Domain);
        using UserPrincipal principal = new(principalContext);
        principal.UserPrincipalName = "*@*";
        principal.Enabled = true;

        using PrincipalSearcher searcher = new(principal);

        return Task.FromResult(
            searcher
                .FindAll()
                .AsQueryable()
                .Cast<UserPrincipal>()
                .FilterUsers()
                .SelectAdUsers()
                .OrderBy(x => x.Surname)
                .ToList()
        );
    }

    public Task<AdUser?> FindDomainUser(string search)
    {
        PrincipalContext principalContext = new(ContextType.Domain);
        UserPrincipal principal = new(principalContext);
        principal.SamAccountName = $"*{search}*";
        principal.Enabled = true;
        PrincipalSearcher searcher = new(principal);

        List<AdUser> users = searcher
            .FindAll()
            .AsQueryable()
            .Cast<UserPrincipal>()
            .FilterUsers()
            .SelectAdUsers()
            .OrderBy(x => x.Surname)
            .ToList();

        return Task.FromResult(users.FirstOrDefault());
    }

    public Task<List<Principal>?> GetGroupsForUser(string samAccountName)
    {
        try
        {
            PrincipalContext principalContext = new(ContextType.Domain);
            var user = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, samAccountName)
                ?? throw new Exception($"Cannot find user by samAccountName: `{samAccountName}`");

            var groups = user.GetGroups().ToList();
            if (groups is null || groups.Count == 0)
            {
                return Task.FromResult<List<Principal>?>(null);
            }
            return Task.FromResult<List<Principal>?>(groups);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{fileName} - [{methodName}]: {exMessage}", FILENAME, nameof(GetGroupsForUser), ex.Message);
            return Task.FromResult<List<Principal>?>(null);
        }
    }

    public Task<bool> IsUserAdmin(string samAccountName, string group = "Domain Admins")
    {
        try
        {
            ArgumentNullException.ThrowIfNullOrEmpty(samAccountName, nameof(samAccountName));
            ArgumentNullException.ThrowIfNullOrEmpty(group, nameof(group));

            return IsUserInGroup(samAccountName, group);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{fileName} - [{methodName}]: {exMessage}", FILENAME, nameof(IsUserAdmin), ex.Message);
            return Task.FromResult(false);
        }
    }

    public Task<bool> IsUserInGroup(string samAccountName, string group)
    {
        try
        {
            ArgumentNullException.ThrowIfNullOrEmpty(samAccountName, nameof(samAccountName));
            ArgumentNullException.ThrowIfNullOrEmpty(group, nameof(group));

            PrincipalContext principalContext = new(ContextType.Domain);

            using GroupPrincipal? gPrincipal = GroupPrincipal.FindByIdentity(principalContext, group)
                ?? throw new InvalidOperationException($"No group found by name: `{group}`");

            using UserPrincipal? uPrincipal = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, samAccountName)
                ?? throw new InvalidOperationException($"No user found by name: `{samAccountName}`");

            return Task.FromResult(uPrincipal.IsMemberOf(gPrincipal));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{fileName} - [{methodName}]: {exMessage}", FILENAME, nameof(IsUserInGroup), ex.Message);
            return Task.FromResult(false);
        }
    }

    public async Task<AdUser> AuthenticateUserAsync(string username, string password)
    {
        try
        {
            ArgumentNullException.ThrowIfNullOrEmpty(username, nameof(username));
            ArgumentNullException.ThrowIfNullOrEmpty(password, nameof(password));

            using PrincipalContext principalContext = new(ContextType.Domain);

            AdUser? user = await FindDomainUser(username)
                ?? throw new InvalidOperationException("No user found. Aborting.");

            if (principalContext.ValidateCredentials(user.SamAccountName!, password))
                return user;
            else
                throw new InvalidOperationException($"Error authenticating user: {username}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{fileName} - [{methodName}]: {exMessage}", FILENAME, nameof(AuthenticateUserAsync), ex.Message);
            throw;
        }
    }
}
