namespace AprovaFacil.Domain.DTOs;

public class NotificationRequest
{
    public Guid RequestUUID { get; init; }
    public Int32[] UsersID { get; init; }
    public String Message { get; set; } = String.Empty;
    public DateTime CreateAt { get; } = DateTime.UtcNow;

    public NotificationRequest(Guid requestUUID, Int32 userId)
    {
        this.RequestUUID = requestUUID;
        this.UsersID = [userId];
        this.Message = "UpdateRequests";
    }

    public NotificationRequest(Guid requestUUID, Int32 userId, String message)
    {
        this.RequestUUID = requestUUID;
        this.UsersID = [userId];
        this.Message = message.Trim();
    }

    public NotificationRequest(Guid requestUUID, Int32[] usersId)
    {
        this.RequestUUID = requestUUID;
        this.UsersID = usersId;
        this.Message = "UpdateRequests";
    }

    public NotificationRequest(Guid requestUUID, Int32[] usersId, String message)
    {
        this.RequestUUID = requestUUID;
        this.UsersID = usersId;
        this.Message = message.Trim();
    }
}

public class NotificationGroupRequest
{
    public Guid RequestUUID { get; init; }
    public String[] Groups { get; init; }
    public String Message { get; set; } = String.Empty;
    public DateTime CreateAt { get; } = DateTime.UtcNow;

    public NotificationGroupRequest(Guid requestUUID, String groups)
    {
        this.RequestUUID = requestUUID;
        this.Groups = [groups];
        this.Message = "UpdateRequests";
    }

    public NotificationGroupRequest(Guid requestUUID, String groups, String message)
    {
        this.RequestUUID = requestUUID;
        this.Groups = [groups];
        this.Message = message.Trim();
    }

    public NotificationGroupRequest(Guid requestUUID, String[] groups)
    {
        this.RequestUUID = requestUUID;
        this.Groups = groups;
        this.Message = "UpdateRequests";
    }

    public NotificationGroupRequest(Guid requestUUID, String[] groups, String message)
    {
        this.RequestUUID = requestUUID;
        this.Groups = groups;
        this.Message = message.Trim();
    }
}