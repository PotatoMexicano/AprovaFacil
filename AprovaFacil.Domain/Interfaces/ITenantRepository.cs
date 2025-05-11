using AprovaFacil.Domain.Models;
using System.Threading.Tasks;

namespace AprovaFacil.Domain.Interfaces;

public interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(int tenantId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);
}

