using AprovaFacil.Domain.Models;
using AprovaFacil.Infra.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AprovaFacil.Infra.Data.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Int32>, Int32>
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Company>().HasKey(c => c.Id);
        builder.Entity<Company>().OwnsOne(Company => Company.Address);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        builder.Entity<IdentityRole<Int32>>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        builder.Entity<IdentityUserRole<Int32>>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });
        });

        builder.Entity<IdentityUserClaim<Int32>>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        builder.Entity<IdentityUserLogin<Int32>>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
        });

        builder.Entity<IdentityUserToken<Int32>>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
        });

        builder.Entity<IdentityRoleClaim<Int32>>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}
