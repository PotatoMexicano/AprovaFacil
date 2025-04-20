using AprovaFacil.Domain.Models;
using AprovaFacil.Infra.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AprovaFacil.Infra.Data.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Int32>, Int32>
{
    public DbSet<Company> Companies { get; set; }

    public DbSet<Request> Requests { get; set; }
    public DbSet<RequestManager> RequestManagers { get; set; }
    public DbSet<RequestDirector> RequestDirectors { get; set; }

    public DbSet<Notification> Notifications { get; set; }

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

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.HasMany(x => x.Notifications)
            .WithOne(x => (ApplicationUser)x.ApplicationUser)
            .HasPrincipalKey(x => x.Id)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Notification>(entity =>
        {
            entity.HasKey(x => x.UUID);
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

        builder.Entity<Request>()
            .HasMany(x => x.Notifications).WithOne()
            .HasForeignKey(x => x.RequestUUID)
            .HasPrincipalKey(x => x.UUID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Request>()
            .HasKey(r => r.UUID);

        builder.Entity<Request>()
            .HasOne(r => r.Company)
            .WithMany(rs => rs.Requests)
            .HasForeignKey(r => r.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Request>()
            .HasOne(r => (ApplicationUser)r.Requester)
            .WithMany(rs => rs.Requests)
            .HasForeignKey(r => r.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuração da tabela RequestGerentes
        builder.Entity<RequestManager>()
            .HasKey(rg => new { rg.RequestUUID, rg.ManagerId });

        builder.Entity<RequestManager>()
            .HasOne(rg => rg.Request)
            .WithMany(r => r.Managers)
            .HasForeignKey(rg => rg.RequestUUID)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete aqui

        builder.Entity<RequestManager>()
            .HasOne(rg => (ApplicationUser)rg.User)
            .WithMany(u => u.RequestManagers)
            .HasForeignKey(rg => rg.ManagerId)
            .OnDelete(DeleteBehavior.Restrict); // Sem cascade para ApplicationUser

        // Configuração da tabela RequestDiretores
        builder.Entity<RequestDirector>()
            .HasKey(rd => new { rd.RequestUUID, rd.DirectorId });

        builder.Entity<RequestDirector>()
            .HasOne(rd => rd.Request)
            .WithMany(r => r.Directors)
            .HasForeignKey(rd => rd.RequestUUID)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete aqui

        builder.Entity<RequestDirector>()
            .HasOne(rd => (ApplicationUser)rd.User)
            .WithMany(u => u.RequestDirectors)
            .HasForeignKey(rd => rd.DirectorId)
            .OnDelete(DeleteBehavior.Restrict); // Sem cascade para ApplicationUser
    }
}
