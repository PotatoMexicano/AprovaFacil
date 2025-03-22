namespace AprovaFacil.Server.DTOs;

public class HttpRequest
{
    public required Int32[] ManagersId { get; set; } = [];
    public required Int32[] DirectorsIds { get; set; } = [];
    public required Int32 CompanyId { get; set; }
    public required DateTime PaymentDate { get; set; }
    public required Int64 Amount { get; set; }
    public String? Note { get; set; }
    public IFormFile? Invoice { get; set; }
    public IFormFile? Budget { get; set; }
}
