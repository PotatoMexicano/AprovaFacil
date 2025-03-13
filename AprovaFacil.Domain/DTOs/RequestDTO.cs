namespace AprovaFacil.Domain.DTOs;

public class RequestDTO
{
    public Int32 Id { get; set; }
    public Int32 ManagerId { get; set; }
    public Int32[] DirectorsIds { get; set; } = [];
    public Int32 CompanyId { get; set; }
    public DateTime PaymentDate { get; set; }
    public Int64 Amount { get; set; }
    public String? Note { get; set; }
    public Byte[] Invoice { get; set; } = [];
    public Byte[] Budget { get; set; } = [];
}
