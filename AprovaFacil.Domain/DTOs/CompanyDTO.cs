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
}

