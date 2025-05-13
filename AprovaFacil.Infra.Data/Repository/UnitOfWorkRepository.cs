using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Infra.Data.Context;

namespace AprovaFacil.Infra.Data.Repository;

public class UnitOfWorkRepository(ApplicationDbContext context) : IUnitOfWorkInterface
{

    public async Task SaveChangesAsync(CancellationToken cancellation)
    {
        await context.SaveChangesAsync(cancellation);
    }

}
