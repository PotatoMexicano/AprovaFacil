using AprovaFacil.Domain.Constants;

namespace AprovaFacil.Domain.Models;

public class Tenant
{
    public Int32 Id { get; set; }

    public required String Name { get; set; }
    public required String Email { get; set; }
    public required String PhoneNumber { get; set; }
    public required String CNPJ { get; set; }

    public required Address Address { get; set; }
    public required String ContactPerson { get; set; }

    public Boolean Active { get; set; } = true;

    public PlanType Plan { get; set; } = PlanType.Basic;

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime SubscriptionEnd { get; set; } = DateTime.UtcNow.AddMonths(1);

    public List<Company> Companies { get; set; } = new();
    public List<Request> Requests { get; set; } = new();
}
