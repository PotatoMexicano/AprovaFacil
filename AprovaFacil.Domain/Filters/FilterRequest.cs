namespace AprovaFacil.Domain.Filters;

public class FilterRequest
{
    public Int32? ApplicationUserId { get; set; }
    public String UserRole { get; set; } = String.Empty;
    public Int32[] Levels { get; set; } = [];
}

