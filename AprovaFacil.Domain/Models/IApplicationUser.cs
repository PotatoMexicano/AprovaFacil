﻿namespace AprovaFacil.Domain.Models;

public interface IApplicationUser
{
    Int32 Id { get; set; }
    String UserName { get; set; }
    String FullName { get; set; }
    String Role { get; set; }
    String Department { get; set; }
    String PictureUrl { get; set; }
    String Email { get; set; }
    Boolean Enabled { get; set; }
    String SecurityStamp { get; set; }

    public Int32 TenantId { get; set; }
    public Tenant Tenant { get; set; }

    public List<Request> Requests { get; set; }
    public List<Request> RequestsFinished { get; set; }

    public List<RequestManager> RequestManagers { get; set; }
    public List<RequestDirector> RequestDirectors { get; set; }

    public List<Notification> Notifications { get; set; }
}
