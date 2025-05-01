namespace AprovaFacil.Domain.Models;

public class Company
{
    public Int32 Id { get; set; }

    public required String CNPJ { get; set; }
    public required String TradeName { get; set; }
    public required String LegalName { get; set; }
    public required Address Address { get; set; }
    public required String Phone { get; set; }
    public required String Email { get; set; }

    public Boolean Enabled { get; set; } = true;

    public required Int32 TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    public List<Request> Requests { get; set; } = new();
}
