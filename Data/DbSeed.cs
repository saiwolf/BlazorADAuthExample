using BlazorADAuth.Entities.Auth;
using BlazorADAuth.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace BlazorADAuth.Data;

public static class DbSeed
{
    public static void SeedDb(ApplicationDbContext context,
        AppUserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        try 
        {
            Log.Information("---Starting DbSeed Routine---");
            Log.Information("---Dropping and Creating Database.");
            context.Database.EnsureDeleted();
            context.Database.Migrate();

            Log.Information("---Creating Roles");
            CreateRoles(roleManager);

            Log.Information("---Creating Admin Users");
            var adminUsers = CreateAdminUsers(userManager);

            Log.Information("---Adding Admin Users to Admin Role");
            AddAdminsToRoles(adminUsers, userManager);

            Log.Information("---DBSeed Routine done!---");
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            Environment.Exit(-1);
        }
    }

    private static void AddAdminsToRoles(List<ApplicationUser> adminUsers, AppUserManager<ApplicationUser> userManager)
    {
        try
        {
            if (adminUsers.IsNullOrEmpty() || adminUsers.Count == 0)
                throw new Exception("Empty user list!");

            foreach (var user in adminUsers)
            {
                var result = userManager.AddToRoleAsync(user, "Admin").Result;
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        throw new Exception(error.Description);
                }
                Log.Information("Added {userName} to Admin Role", user.DisplayName);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            throw;
        }
    }

    private static List<ApplicationUser> CreateAdminUsers(AppUserManager<ApplicationUser> userManager)
    {
        try
        {
            ApplicationUser[] adminUsers =
            [
                new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "admin-user",
                    Email = "admin@your-domain",
                    DisplayName = "Admin User",
                    Created = DateTime.Now,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = false,
                },
                // add more admins as needed
            ];

            foreach (var user in adminUsers)
            {
                var result = userManager.CreateAsync(user).Result;
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        throw new Exception(error.Description);
                }
            }

            return [.. userManager.Users];
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            throw;
        }
    }

    private static void CreateRoles(RoleManager<ApplicationRole> roleManager)
    {
        try
        {
            List<ApplicationRole> roles = [
                new ApplicationRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Created = DateTime.Now,
                },
                new ApplicationRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "User",
                    NormalizedName = "USER",
                    Created = DateTime.Now,
                }
            ];

            foreach (var role in roles)
            {
                var result = roleManager.CreateAsync(role).Result;
                if (!result.Succeeded)
                    throw new Exception(result.Errors.First().Description);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            throw;
        }
    }
}
