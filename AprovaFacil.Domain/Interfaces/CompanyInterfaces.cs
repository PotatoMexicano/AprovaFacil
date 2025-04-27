using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Models;
using AprovaFacil.Domain.Results;

namespace AprovaFacil.Domain.Interfaces;

public static class CompanyInterfaces
{
    public interface ICompanyService
    {
        Task<Result<CompanyDTO[]>> GetAllCompanies(CancellationToken cancellation);
        Task<Result<CompanyDTO>> GetCompany(Int64 idCompany, CancellationToken cancellation);

        Task<Result<CompanyDTO>> RegisterCompany(CompanyDTO request, CancellationToken cancellation);
        Task<Result<CompanyDTO>> UpdateCompany(CompanyDTO request, CancellationToken cancellation);
        Task<Result> DeleteCompany(Int32 id, CancellationToken cancellation);
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
