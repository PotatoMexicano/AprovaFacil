using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Models;

namespace AprovaFacil.Domain.Extensions;

public static class CompanyExtensions
{
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
