using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AprovaFacil.Infra.Data.Repository;

public class CompanyRepository(ApplicationDbContext context) : CompanyInterfaces.ICompanyRepository
{
    public async Task DeleteCompanyAsync(Company request, CancellationToken cancellation)
    {
        context.Companies.Update(request);
        await context.SaveChangesAsync(cancellation);
    }

    public async Task<Company[]> GetAllCompaniesAsync(Int32 tenantId, CancellationToken cancellation)
    {
        IQueryable<Company> query = context.Companies.Where(c => c.Enabled && c.TenantId == tenantId);
        return await query.ToArrayAsync();
    }

    public async Task<Company?> GetCompanyAsync(Int64 idCompany, Int32 tenantId, CancellationToken cancellation)
    {
        Company? company = await context.Companies.Where(c => c.Id == idCompany && c.TenantId == tenantId).FirstOrDefaultAsync();
        return company;
    }

    public async Task<Company?> RegisterCompanyAsync(Company request, CancellationToken cancellation)
    {
        context.Companies.Add(request);
        await context.SaveChangesAsync(cancellation);
        return request;
    }

    public async Task<Company?> UpdateCompanyAsync(Company request, CancellationToken cancellation)
    {
        context.Companies.Update(request);
        await context.SaveChangesAsync(cancellation);
        return request;
    }
}

