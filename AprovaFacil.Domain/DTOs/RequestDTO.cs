namespace AprovaFacil.Domain.DTOs;

public class RequestDTO
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

    public UserDTO Requester { get; set; }
    public List<UserDTO> Managers { get; set; } = new();
    public List<UserDTO> Directors { get; set; } = new();
}

public class RequestRegisterDTO
{
    public Guid UUID { get; private set; } = Guid.NewGuid();

    public Int32 RequesterId { get; set; }

    public Int32[] ManagersId { get; set; } = [];
    public Int32[] DirectorsIds { get; set; } = [];

    public Int32 CompanyId { get; set; }

    public DateTime PaymentDate { get; set; }

    public Int64 Amount { get; set; }
    public String? Note { get; set; }

    public DateTime CreateAt { get; } = DateTime.UtcNow;

    public String? InvoiceFileName { get; set; }
    public Byte[] Invoice { get; set; } = [];

    public String? BudgetFileName { get; set; }
    public Byte[] Budget { get; set; } = [];
}
