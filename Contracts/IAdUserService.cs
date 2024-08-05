using BlazorADAuth.Entities.Auth;
using System.Runtime.Versioning;
using System.Security.Principal;

namespace BlazorADAuth.Contracts;

public interface IAdUserService
{
    Task<AdUser> GetAdUser(IIdentity identity);
    Task<AdUser> GetAdUser(string samAccountName);
    Task<AdUser> GetAdUser(Guid guid);
    Task<List<AdUser>> GetDomainUsers();
    Task<AdUser?> FindDomainUser(string search);
    Task<bool> IsUserAdmin(string samAccountName, string group = "Domain Admins");
    Task<bool> IsUserInGroup(string samAccountName, string group);
    Task<AdUser> AuthenticateUserAsync(string username, string password);
}
