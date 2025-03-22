using AprovaFacil.Application.Extensions;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using Serilog;

namespace AprovaFacil.Application.Services;

public class CompanyService : CompanyInterfaces.ICompanyService
{
    private readonly CompanyInterfaces.ICompanyRepository _repository;
    private readonly ILogger _logger;

    public CompanyService(CompanyInterfaces.ICompanyRepository repository, ILogger logger)
    {
        this._repository = repository;
        this._logger = logger;

    }

    public async Task DeleteCompany(Int32 id, CancellationToken cancellation)
    {
        Company? company = await _repository.GetCompanyAsync(id, cancellation);

        if (company is null) return;

        company.Enabled = false;

        await _repository.DeleteCompanyAsync(company, cancellation);

        _logger.Information("Company {Id} deleted", id);

        return;
    }

    public async Task<CompanyDTO[]> GetAllCompanies(CancellationToken cancellation)
    {
        Company[] companies = await _repository.GetAllCompaniesAsync(cancellation);
        CompanyDTO[] dtos = companies.Select(CompanyExtensions.ToDTO).ToArray();
        return dtos;
    }

    public async Task<CompanyDTO?> GetCompany(Int64 idCompany, CancellationToken cancellation)
    {
        Company? company = await _repository.GetCompanyAsync(idCompany, cancellation);
        CompanyDTO? dto = company?.ToDTO();
        return dto;
    }

    public async Task<CompanyDTO?> RegisterCompany(CompanyDTO request, CancellationToken cancellation)
    {
        Company company = request.ToEntity();

        Company? registeredCompany = await _repository.RegisterCompanyAsync(company, cancellation);

        if (registeredCompany is null) return null;

        _logger.Information("Company {Id} registered", registeredCompany.Id);

        CompanyDTO dto = registeredCompany.ToDTO();

        return dto;
    }

    public async Task<CompanyDTO?> UpdateCompany(CompanyDTO request, CancellationToken cancellation)
    {
        Company? companyEntity = await _repository.GetCompanyAsync(request.Id, cancellation);

        if (companyEntity is null) return null;

        companyEntity = request.UpdateEntity(companyEntity);

        Company? updatedCompany = await _repository.UpdateCompanyAsync(companyEntity, cancellation);

        if (updatedCompany is null) return null;

        _logger.Information("Company {Id} updated", updatedCompany.Id);

        CompanyDTO dto = updatedCompany.ToDTO();

        return dto;
    }
}
