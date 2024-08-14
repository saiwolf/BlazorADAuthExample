using BlazorADAuth.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;

namespace BlazorADAuth.Helpers;

[SupportedOSPlatform("windows")]
public static class IdentityExtensions
{
    public static IQueryable<UserPrincipal> FilterUsers(this IQueryable<UserPrincipal> principals) =>
        principals.Where(x => x.Guid.HasValue);
    public static IQueryable<AdUser> SelectAdUsers(this IQueryable<UserPrincipal> principals) =>
        principals.Select(x => AdUser.CastToAdUser(x));    
}
