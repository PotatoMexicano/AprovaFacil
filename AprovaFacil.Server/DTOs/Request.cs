namespace AprovaFacil.Server.DTOs;

public class HttpRequest
{
    public Int32 ManagerId { get; set; }
    public Int32[]? DirectorsIds { get; set; }
    public Int32 CompanyId { get; set; }
    public DateTime PaymentDate { get; set; }
    public Int64 Amount { get; set; }
    public String? Note { get; set; }
    public IFormFile? Invoice { get; set; }
    public IFormFile? Budget { get; set; }
}
