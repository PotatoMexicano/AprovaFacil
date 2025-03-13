using AprovaFacil.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace AprovaFacil.Infra.Data.Identity;

public class ApplicationUser : IdentityUser<Int32>, IApplicationUser
{
    public String FullName { get; set; }
    public String Role { get; set; } // Propriedade de dados, não a role do Identity
    public String Department { get; set; }
    public String PictureUrl { get; set; }
    public Boolean Enabled { get; set; }
}
