namespace AprovaFacil.Domain.DTOs;

public class ServerDirectory
{
    public required String InvoicePath { get; init; }
    public required String BudgetPath { get; init; }
}

public class MinioSettings
{
    public String Endpoint { get; set; } = String.Empty;
    public String AccessKey { get; set; } = String.Empty;
    public String SecretKey { get; set; } = String.Empty;
    public Boolean WithSSL { get; set; }
}
