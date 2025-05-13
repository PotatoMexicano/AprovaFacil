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

    public PlanType Plan { get; set; } = PlanType.Free;

    public Int32 MaxRequestsPerMonth { get; private set; }
    public Int32 MaxUsers { get; private set; }

    public Int32 CurrentRequestsThisMonth { get; set; } = 0;
    public Int32 CurrentUserCount { get; set; } = 0;
    public DateTime LastRequestResetDate { get; set; }

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime SubscriptionEnd { get; set; } = DateTime.UtcNow.AddMonths(1);

    public List<Company> Companies { get; set; } = new();
    public List<Request> Requests { get; set; } = new();

    public Tenant()
    {
        SetLimitsBasedOnPlan();

        if (this.LastRequestResetDate == default(DateTime))
        {
            this.LastRequestResetDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        }
    }

    public void SetLimitsBasedOnPlan()
    {
        switch (this.Plan)
        {
            case PlanType.Free:
                this.MaxRequestsPerMonth = 10;
                this.MaxUsers = 5;
                break;
            case PlanType.Basic:
                this.MaxRequestsPerMonth = 30;
                this.MaxUsers = 7;
                break;
            case PlanType.Intermidiate:
                this.MaxRequestsPerMonth = 100;
                this.MaxUsers = 20;
                break;
            case PlanType.Business:
                this.MaxRequestsPerMonth = Int32.MaxValue; // Represents unlimited
                this.MaxUsers = 50;
                break;
            default:
                this.MaxRequestsPerMonth = 10;
                this.MaxUsers = 5;
                break;
        }
    }

    public void UpdatePlan(PlanType newPlan)
    {
        this.Plan = newPlan;
        SetLimitsBasedOnPlan();
        // Optionally reset current counts or handle pro-rata logic here if needed
        // For simplicity, we'll assume counts are not reset immediately on plan change
        // but the new limits apply going forward.
    }
}

