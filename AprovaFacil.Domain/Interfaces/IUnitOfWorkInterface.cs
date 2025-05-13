namespace AprovaFacil.Domain.Interfaces;

public interface IUnitOfWorkInterface
{
    Task SaveChangesAsync(CancellationToken cancellation);
    Task Rollback();
}
