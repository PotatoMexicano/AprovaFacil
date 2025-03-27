using AprovaFacil.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace AprovaFacil.Infra.Data.Identity;

public class ApplicationUser : IdentityUser<Int32>, IApplicationUser
{
    public String FullName { get; set; } = null!;
    public String Role { get; set; } = null!; // Propriedade de dados, não a role do Identity 
    public String Department { get; set; } = null!;
    public String PictureUrl { get; set; } = null!;
    public Boolean Enabled { get; set; }

    public override String UserName
    {
        get => base.UserName!;
        set => base.UserName = value;
    }

    public override String Email
    {
        get => base.Email!;
        set => base.Email = value;
    }

    public override String SecurityStamp
    {
        get => base.SecurityStamp!;
        set => base.SecurityStamp = value;
    }

    public List<Request> Requests { get; set; } = new();
    public List<RequestManager> RequestManagers { get; set; } = new();
    public List<RequestDirector> RequestDirectors { get; set; } = new();
}
