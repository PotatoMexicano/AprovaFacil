namespace AprovaFacil.Domain.DTOs;

public class NotificationRequestDTO
{
    public required String RequestUUID { get; set; }
    public required Int32 RequesterID { get; set; }
    public IEnumerable<Int32> ManagerIds { get; set; } = [];
    public IEnumerable<Int32> DirectorIds { get; set; } = [];
}
