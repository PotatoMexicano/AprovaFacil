namespace AprovaFacil.Domain.Models;

public class Request
{
    public Guid UUID { get; set; }

    public Int32 RequesterId { get; set; }

    public Guid InvoiceName { get; set; }
    public Guid BudgetName { get; set; }

    public DateTime? PaymentDate { get; set; }
    public DateTime CreateAt { get; set; }

    public DateTime? ApprovedFirstLevelAt { get; set; }
    public DateTime? ApprovedSecondLevelAt { get; set; }

    public DateTime? ReceivedAt { get; set; }

    public Int64 Amount { get; set; }
    public String? Note { get; set; }

    public IApplicationUser Requester { get; set; }
    public List<RequestManager> Managers { get; set; } = new();
    public List<RequestDirector> Directors { get; set; } = new();
}

public class RequestManager
{
    public Guid RequestUUID { get; set; }
    public Int32 ManagerId { get; set; }

    public Request Request { get; set; }
    public IApplicationUser User { get; set; }
}

public class RequestDirector
{
    public Guid RequestUUID { get; set; }
    public Int32 DirectorId { get; set; }

    public Request Request { get; set; }
    public IApplicationUser User { get; set; }
}