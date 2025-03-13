using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Models;

namespace AprovaFacil.Application.Extensions;

public static class CompanyExtension
{
    public static CompanyDTO ToDTO(this Company company)
    {
        return new CompanyDTO
        {
            Id = company.Id,
            TradeName = company.TradeName,
            City = company.Address.City,
            PostalCode = company.Address.PostalCode,
            State = company.Address.State,
            Street = company.Address.Street,
            Complement = company.Address.Complement,
            Neighborhood = company.Address.Neighborhood,
            Number = company.Address.Number,
            Phone = company.Phone,
            Email = company.Email,
            LegalName = company.LegalName,
            CNPJ = company.CNPJ,
        };
    }

    public static Company ToEntity(this CompanyDTO request)
    {
        return new Company
        {
            TradeName = request.TradeName,
            CNPJ = request.CNPJ,
            Email = request.Email,
            Phone = request.Phone,
            LegalName = request.LegalName,
            Address = new Address
            {
                City = request.City,
                Complement = request.Complement,
                Neighborhood = request.Neighborhood,
                Number = request.Number,
                PostalCode = request.PostalCode,
                State = request.State,
                Street = request.Street
            }
        };
    }

    public static Company UpdateEntity(this CompanyDTO request, Company company)
    {
        company.CNPJ = request.CNPJ;
        company.TradeName = request.TradeName;
        company.LegalName = request.LegalName;
        company.Address = new Address
        {
            PostalCode = request.PostalCode,
            State = request.State,
            City = request.City,
            Neighborhood = request.Neighborhood,
            Street = request.Street,
            Number = request.Number,
            Complement = request.Complement,
        };
        company.Phone = request.Phone;
        company.Email = request.Email;

        return company;
    }
}
