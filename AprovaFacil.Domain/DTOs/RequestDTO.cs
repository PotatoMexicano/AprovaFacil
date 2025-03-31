namespace AprovaFacil.Domain.DTOs;

public class RequestDTO
{
    public String UUID { get; set; } = null!;

    public Int32 RequesterId { get; set; }

    public String InvoiceName { get; set; } = null!;
    public String BudgetName { get; set; } = null!;
    public Boolean HasInvoice { get; set; }
    public Boolean HasBudget { get; set; }

    public DateTime? PaymentDate { get; set; }
    public DateTime CreateAt { get; set; }

    public DateTime? FirstLevelAt { get; set; }
    public Boolean ApprovedFirstLevel { get; set; }
    public DateTime? SecondLevelAt { get; set; }
    public Boolean ApprovedSecondLevel { get; set; }

    public DateTime? ReceivedAt { get; set; }

    public Int32 Approved
    {
        get
        {
            if (FirstLevelAt.HasValue && !ApprovedFirstLevel)
            {
                return -1;
            }

            if (SecondLevelAt.HasValue && !ApprovedSecondLevel)
            {
                return -1;
            }

            if (FirstLevelAt.HasValue && SecondLevelAt.HasValue)
            {
                if (ApprovedFirstLevel && ApprovedSecondLevel)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            return 0;
        }
    }

    public Int64 Amount { get; set; }
    public String? Note { get; set; }

    public CompanyDTO Company { get; set; } = null!;
    public UserDTO Requester { get; set; } = null!;
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
