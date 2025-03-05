using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Models;

namespace AprovaFacil.Domain.Interfaces;

public static class CompanyInterfaces
{
    public interface ICompanyService
    {
        Task<CompanyDTO[]> GetAllCompanies(CancellationToken cancellation);
        Task<CompanyDTO?> GetCompany(Int64 idCompany, CancellationToken cancellation);

        Task<CompanyDTO?> RegisterCompany(CompanyDTO request, CancellationToken cancellation);
        Task<CompanyDTO?> UpdateCompany(CompanyDTO request, CancellationToken cancellation);
        Task DeleteCompany(Int32 id, CancellationToken cancellation);
    }

    public interface ICompanyRepository
    {
        Task<Company[]> GetAllCompaniesAsync(CancellationToken cancellation);
        Task<Company?> GetCompanyAsync(Int64 idCompany, CancellationToken cancellation);

        Task<Company?> RegisterCompanyAsync(Company request, CancellationToken cancellation);
        Task<Company?> UpdateCompanyAsync(Company request, CancellationToken cancellation);
        Task DeleteCompanyAsync(Company request, CancellationToken cancellation);
    }
}
