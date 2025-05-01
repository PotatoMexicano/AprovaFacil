using AprovaFacil.Domain.Models;

namespace AprovaFacil.Domain.DTOs;

public class CompanyDTO
{
    public Int32 Id { get; set; }
    public String CNPJ { get; set; }
    public String TradeName { get; set; }
    public String LegalName { get; set; }
    public String PostalCode { get; set; }

    public String State { get; set; }
    public String City { get; set; }
    public String Neighborhood { get; set; }
    public String Street { get; set; }
    public String Number { get; set; }
    public String Complement { get; set; }

    public String Phone { get; set; }
    public String Email { get; set; }

    public Int32 TenantId { get; set; }

    public static implicit operator CompanyDTO?(Company? company)
    {
        if (company is null) return null;

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
            TenantId = company.TenantId,
        };
    }

    public static implicit operator Company?(CompanyDTO? company)
    {
        if (company is null) return null;

        return new Company
        {
            TradeName = company.TradeName,
            CNPJ = company.CNPJ,
            Email = company.Email,
            Phone = company.Phone,
            LegalName = company.LegalName,
            TenantId = company.TenantId,
            Address = new Address
            {
                City = company.City,
                Complement = company.Complement,
                Neighborhood = company.Neighborhood,
                Number = company.Number,
                PostalCode = company.PostalCode,
                State = company.State,
                Street = company.Street
            }
        };
    }
}

