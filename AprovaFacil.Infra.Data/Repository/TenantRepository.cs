using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace AprovaFacil.Infra.Data.Repository;

public class TenantRepository : ITenantRepository
{
    private readonly ApplicationDbContext _context;

    public TenantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    private Int32 CountUsersEnabled(Int32 tenantId)
    {
        Int32 count = _context.Users.Where(u => u.TenantId == tenantId && u.Enabled == true).Count();
        return count;
    }

    public async Task<Tenant?> GetByIdAsync(Int32 tenantId, CancellationToken cancellation)
    {
        Tenant? result = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, cancellation);

        if (result == null)
        {
            return result;
        }

        result.CurrentUserCount = this.CountUsersEnabled(tenantId);

        return result;
    }

    public Task UpdateAsync(Tenant tenant, CancellationToken cancellation)
    {
        _context.Tenants.Update(tenant);
        return Task.CompletedTask;
    }
}

