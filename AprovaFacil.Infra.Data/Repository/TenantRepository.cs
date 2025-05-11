using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AprovaFacil.Infra.Data.Repository;

public class TenantRepository : ITenantRepository
{
    private readonly ApplicationDbContext _context;

    public TenantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Tenant?> GetByIdAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
    }

    public async Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        _context.Tenants.Update(tenant);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

