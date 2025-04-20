namespace AprovaFacil.Domain.Models;

public class Notification
{
    public Guid UUID { get; set; }

    public Guid RequestUUID { get; set; }
    public Int32 UserId { get; set; }

    public String Message { get; set; } = null!;
    public DateTime CreateAt { get; set; }
    public DateTime ExpireAt { get; set; }

    public Boolean Opened { get; set; }

    public IApplicationUser ApplicationUser { get; set; } = null!;
}
