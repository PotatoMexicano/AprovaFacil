using AprovaFacil.Domain.Constants;
using System;
using System.Collections.Generic;

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

    // New properties for plan limits and usage tracking
    public int MaxRequestsPerMonth { get; private set; }
    public int MaxUsers { get; private set; }
    public int CurrentRequestsThisMonth { get; set; } = 0;
    public int CurrentUserCount { get; set; } = 0;
    public DateTime LastRequestResetDate { get; set; }

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime SubscriptionEnd { get; set; } = DateTime.UtcNow.AddMonths(1);

    public List<Company> Companies { get; set; } = new();
    public List<Request> Requests { get; set; } = new();

    // Constructor to initialize limits based on plan
    public Tenant()
    {
        SetLimitsBasedOnPlan();
        // Initialize reset date if it's a new tenant or first time setting limits
        if (this.LastRequestResetDate == default(DateTime))
        {
            this.LastRequestResetDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        }
    }

    // Method to set limits based on plan type - should be called when plan is set or changed, or tenant is loaded.
    public void SetLimitsBasedOnPlan()
    {
        switch (this.Plan)
        {
            case PlanType.Gratis:
                this.MaxRequestsPerMonth = 10;
                this.MaxUsers = 5;
                break;
            case PlanType.Basico:
                this.MaxRequestsPerMonth = 30;
                this.MaxUsers = 7;
                break;
            case PlanType.Intermediario:
                this.MaxRequestsPerMonth = 100;
                this.MaxUsers = 20;
                break;
            case PlanType.Business:
                this.MaxRequestsPerMonth = int.MaxValue; // Represents unlimited
                this.MaxUsers = 50;
                break;
            default:
                // Default to a restricted plan (e.g., free or no access) or throw an exception
                this.MaxRequestsPerMonth = 10; // Defaulting to Gratis limits as a fallback
                this.MaxUsers = 5;
                break;
        }
    }

    // Call this method when the plan is changed
    public void UpdatePlan(PlanType newPlan)
    {
        this.Plan = newPlan;
        SetLimitsBasedOnPlan();
        // Optionally reset current counts or handle pro-rata logic here if needed
        // For simplicity, we'll assume counts are not reset immediately on plan change
        // but the new limits apply going forward.
    }
}

