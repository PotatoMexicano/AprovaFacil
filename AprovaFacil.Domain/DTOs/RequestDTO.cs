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
    public Int32 ApprovedFirstLevel
    {
        get
        {
            if (!this.Managers.Any()) // Se não há gerentes, está pendente
            {
                return 0;
            }

            Boolean hasAccepted = this.Managers.Any(m => m.RequestApproved == 1);
            Boolean hasDenied = this.Managers.Any(m => m.RequestApproved == -1);
            Boolean allDenied = this.Managers.All(m => m.RequestApproved == -1);

            if (hasAccepted) // Se pelo menos um aceitou, retorna aceito
            {
                return 1;
            }
            else if (allDenied) // Se todos recusaram, retorna recusado
            {
                return -1;
            }
            else // Se não há aceites nem todos recusaram, está pendente
            {
                return 0;
            }
        }
    }
    public DateTime? SecondLevelAt { get; set; }
    public Int32 ApprovedSecondLevel
    {
        get
        {
            if (!this.Directors.Any()) // Se não há diretores, está pendente
            {
                return 0;
            }

            Boolean hasAccepted = this.Directors.Any(d => d.RequestApproved == 1);
            Boolean hasDenied = this.Directors.Any(d => d.RequestApproved == -1);
            Boolean allDenied = this.Directors.All(d => d.RequestApproved == -1);

            if (hasAccepted) // Se pelo menos um aceitou, retorna aceito
            {
                return 1;
            }
            else if (allDenied) // Se todos recusaram, retorna recusado
            {
                return -1;
            }
            else // Se não há aceites nem todos recusaram, está pendente
            {
                return 0;
            }
        }
    }

    public DateTime? ReceivedAt { get; set; }

    public Int32 Approved
    {
        get
        {
            Boolean hasDirectors = this.Directors != null && this.Directors.Any();

            // Se foi recusado em qualquer nível, retorna recusado
            if (this.ApprovedFirstLevel == -1 || this.ApprovedSecondLevel == -1)
            {
                return -1;
            }

            // Se foi aprovado no primeiro nível e não há diretores, retorna aprovado
            if (this.ApprovedFirstLevel == 1 && !hasDirectors)
            {
                return 1;
            }

            // Se foi aprovado no primeiro nível e o segundo nível está pendente, retorna pendente
            if (this.ApprovedFirstLevel == 1 && this.ApprovedSecondLevel == 0)
            {
                return 0;
            }

            // Se foi aprovado nos dois níveis, retorna aprovado
            if (this.ApprovedFirstLevel == 1 && this.ApprovedSecondLevel == 1)
            {
                return 1;
            }

            // Para todos os outros casos (ex.: primeiro nível pendente), retorna pendente
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
