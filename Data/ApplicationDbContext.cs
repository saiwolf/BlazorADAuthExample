using BlazorADAuth.Entities.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BlazorADAuth.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>().Property(p => p.FriendlyId)
            .ValueGeneratedOnAdd().UseIdentityColumn(1000, 1);

        modelBuilder.Entity<ApplicationUser>()
            .HasAlternateKey(a => a.FriendlyId);
    }
}
