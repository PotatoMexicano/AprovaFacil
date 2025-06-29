﻿using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Extensions;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Domain.Results;
using Serilog;

namespace AprovaFacil.Application.Services;

public class CompanyService(CompanyInterfaces.ICompanyRepository repository, ITenantProvider tenant) : CompanyInterfaces.ICompanyService
{
    public async Task<Result> DeleteCompany(Int32 id, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenant.GetTenantId();

            if (!tenantId.HasValue) return Result.Failure(ErrorType.NotFound, "TenantId não encontrado.");

            Company? company = await repository.GetCompanyAsync(id, tenantId.Value, cancellation);

            if (company is null) return Result.Failure(ErrorType.InternalError, "Não foi possível deletar a empresa.");

            company.Enabled = false;

            await repository.DeleteCompanyAsync(company, cancellation);

            return Result.Success();
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result.Failure(ErrorType.InternalError, "Ocorreu um erro ao deletar a empresa.");
        }
    }

    public async Task<Result<CompanyDTO[]>> GetAllCompanies(CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenant.GetTenantId();

            if (!tenantId.HasValue) return Result<CompanyDTO[]>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            Company[] companies = await repository.GetAllCompaniesAsync(tenantId.Value, cancellation);

            if (companies is null) return Result<CompanyDTO[]>.Failure(ErrorType.NotFound, "Nenhuma empresa encontrada.");

            CompanyDTO[] result = companies.Select<Company, CompanyDTO>(x => x).ToArray();

            return Result<CompanyDTO[]>.Success(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<CompanyDTO[]>.Failure(ErrorType.InternalError, "Ocorreu um erro ao buscar empresas.");
        }
    }

    public async Task<Result<CompanyDTO>> GetCompany(Int64 idCompany, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenant.GetTenantId();

            if (!tenantId.HasValue) return Result<CompanyDTO>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            Company? company = await repository.GetCompanyAsync(idCompany, tenantId.Value, cancellation);

            if (company is null) return Result<CompanyDTO>.Failure(ErrorType.NotFound, "Nenhuma empresa encontrada.");

            CompanyDTO? result = company;

            if (result is null) return Result<CompanyDTO>.Failure(ErrorType.Validation, "Falha ao validar empresa.");

            return Result<CompanyDTO>.Success(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<CompanyDTO>.Failure(ErrorType.InternalError, "Ocorreu um erro ao buscar empresa.");
        }
    }

    public async Task<Result<CompanyDTO>> RegisterCompany(CompanyDTO request, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenant.GetTenantId();

            if (!tenantId.HasValue) return Result<CompanyDTO>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            Company company = new Company
            {
                TradeName = request.TradeName,
                CNPJ = request.CNPJ,
                Email = request.Email,
                LegalName = request.LegalName,
                Phone = request.Phone,
                TenantId = tenantId.Value,
                Address = new Address
                {
                    City = request.City,
                    Complement = request.Complement,
                    Neighborhood = request.Neighborhood,
                    Number = request.Number,
                    PostalCode = request.PostalCode,
                    State = request.State,
                    Street = request.Street,
                }
            };

            if (company is null) return Result<CompanyDTO>.Failure(ErrorType.Validation, "Falha ao validar requisição.");

            Company? registeredCompany = await repository.RegisterCompanyAsync(company, cancellation);

            if (registeredCompany is null) return Result<CompanyDTO>.Failure(ErrorType.NotFound, "Não foi possível registrar a empresa.");

            CompanyDTO? result = registeredCompany;

            if (result is null) return Result<CompanyDTO>.Failure(ErrorType.Validation, "Falha ao validar empresa.");

            return Result<CompanyDTO>.Success(result);

        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<CompanyDTO>.Failure(ErrorType.InternalError, "Ocorreu um erro ao registrar empresa.");
        }
    }

    public async Task<Result<CompanyDTO>> UpdateCompany(CompanyDTO request, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenant.GetTenantId();

            if (!tenantId.HasValue) return Result<CompanyDTO>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            Company? companyEntity = await repository.GetCompanyAsync(request.Id, tenantId.Value, cancellation);

            if (companyEntity is null) return Result<CompanyDTO>.Failure(ErrorType.NotFound, "Nenhuma empresa encontrada.");

            companyEntity = request.UpdateEntity(companyEntity);

            Company? updatedCompany = await repository.UpdateCompanyAsync(companyEntity, cancellation);

            if (updatedCompany is null) return Result<CompanyDTO>.Failure(ErrorType.NotFound, "Não foi possível atualizar a empresa.");

            CompanyDTO? result = updatedCompany;

            if (result is null) return Result<CompanyDTO>.Failure(ErrorType.Validation, "Falha ao validar empresa.");

            return Result<CompanyDTO>.Success(result);

        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<CompanyDTO>.Failure(ErrorType.InternalError, "Ocorreu um erro ao atualizar a empresa.");
        }
    }
}
