﻿namespace AprovaFacil.Domain.Models;

public class Request
{
    public Guid UUID { get; set; }

    public Int32 CompanyId { get; set; }
    public Int32 RequesterId { get; set; }
    public Int32? FinisherId { get; set; }

    public Guid InvoiceName { get; set; }
    public Boolean HasInvoice { get; set; }
    public Guid BudgetName { get; set; }
    public Boolean HasBudget { get; set; }

    public DateTime? PaymentDate { get; set; }
    public DateTime CreateAt { get; set; }

    public DateTime? FirstLevelAt { get; set; }
    public Boolean ApprovedFirstLevel { get; set; }
    public DateTime? SecondLevelAt { get; set; }
    public Boolean ApprovedSecondLevel { get; set; }

    public Int32 Level { get; set; }
    public Int32 Status { get; set; }

    public DateTime? FinishedAt { get; set; }

    public Int64 Amount { get; set; }
    public String? Note { get; set; }

    public Int32 TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    public Company Company { get; set; } = null!;

    public IApplicationUser Requester { get; set; } = null!;
    public IApplicationUser? Finisher { get; set; }

    public List<RequestManager> Managers { get; set; } = new();
    public List<RequestDirector> Directors { get; set; } = new();

    public List<Notification> Notifications { get; set; } = new();
}

public class RequestManager
{
    public Guid RequestUUID { get; set; }
    public Int32 ManagerId { get; set; }

    public Int32 Approved { get; set; }

    public Request Request { get; set; } = null!;
    public IApplicationUser User { get; set; } = null!;
}

public class RequestDirector
{
    public Guid RequestUUID { get; set; }
    public Int32 DirectorId { get; set; }

    public Int32 Approved { get; set; }

    public Request Request { get; set; } = null!;
    public IApplicationUser User { get; set; } = null!;
}