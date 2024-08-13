using Microsoft.AspNetCore.Identity;

namespace BlazorADAuth.Helpers;

public static class IdentityErrorExtensions
{
    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating an invalid AD password.
    /// </summary>
    /// <returns>An <see cref="IdentityError"/> indicating an invalid AD password.</returns>
    public static IdentityError ADInvalidPassword()
    {
        return new IdentityError
        {
            Code = nameof(ADInvalidPassword),
            Description = "Invalid AD password"
        };
    }

    public static IdentityError ADInvalidUserName()
    {
        return new IdentityError
        {
            Code = nameof(ADInvalidUserName),
            Description = "Invalid AD username"
        };
    }

    public static IdentityError AppInvalidUserName()
    {
        return new IdentityError
        {
            Code = nameof(AppInvalidUserName),
            Description = "Invalid App username"
        };
    }
}
