using AprovaFacil.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AprovaFacil.Infra.Data.Context;

public class ApplicationDbContext : DbContext
{
    public DbSet<Company> Companies { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Company>().HasKey(c => c.Id);
        builder.Entity<Company>().OwnsOne(Company => Company.Address);
    }
}
