namespace AprovaFacil.Domain.Models;

public class Company
{
    public Int32 Id { get; set; }
    public String CNPJ { get; set; }
    public String TradeName { get; set; }
    public String LegalName { get; set; }
    public Address Address { get; set; }
    public String Phone { get; set; }
    public String Email { get; set; }

    public Boolean Enabled { get; set; } = true;
}
