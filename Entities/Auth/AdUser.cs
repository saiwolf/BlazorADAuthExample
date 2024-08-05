using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using System.Security.Principal;

namespace BlazorADAuth.Entities.Auth;

[SupportedOSPlatform("windows")]
public class AdUser
{
    public DateTime? AccountExpirationDate { get; set; }
    public DateTime? AccountLockoutTime { get; set; }
    public int BadLogonCount { get; set; }
    public string? Description { get; set; }
    public string? DisplayName { get; set; }
    public string? DistinguishedName { get; set; }
    public string? Domain { get; set; }
    public string? EmailAddress { get; set; }
    //public string EmployeeId { get; set; }
    public bool? Enabled { get; set; }
    public string? GivenName { get; set; }
    public Guid? Guid { get; set; }
    //public string HomeDirectory { get; set; }
    //public string HomeDrive { get; set; }
    //public DateTime? LastBadPasswordAttempt { get; set; }
    //public DateTime? LastLogon { get; set; }
    //public DateTime? LastPasswordSet { get; set; }
    //public string MiddleName { get; set; }
    //public string Name { get; set; }
    //public bool PasswordNeverExpires { get; set; }
    //public bool PasswordNotRequired { get; set; }
    public string? SamAccountName { get; set; }
    //public string ScriptPath { get; set; }
    public SecurityIdentifier? Sid { get; set; }
    public string? Surname { get; set; }
    //public bool UserCannotChangePassword { get; set; }
    public string? UserPrincipalName { get; set; }
    //public string VoiceTelephoneNumber { get; set; }

    public static AdUser CastToAdUser(UserPrincipal user)
    {
        if (user is null)
            return new AdUser();
        return new AdUser
        {
            AccountExpirationDate = user.AccountExpirationDate,
            AccountLockoutTime = user.AccountLockoutTime,
            BadLogonCount = user.BadLogonCount,
            Description = user.Description,
            DisplayName = user.DisplayName,
            DistinguishedName = user.DistinguishedName,
            EmailAddress = user.EmailAddress,            
            Enabled = user.Enabled,
            GivenName = user.GivenName,
            Guid = user.Guid,
            SamAccountName = user.SamAccountName,
            Sid = user.Sid,
            Surname = user.Surname,
            UserPrincipalName = user.UserPrincipalName            
        };
    }

    public string GetDomainPrefix() => DistinguishedName!
        .Split(',')!
        .FirstOrDefault(x => x.ToLower().Contains("dc"))!
        .Split('=')!
        .LastOrDefault()!
        .ToUpper();
}
