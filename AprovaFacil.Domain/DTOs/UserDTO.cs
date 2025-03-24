namespace AprovaFacil.Domain.DTOs;

public class UserDTO
{
    public Int32 Id { get; set; }
    public String FullName { get; set; }

    public String RoleLabel { get; set; }
    public String Role { get; set; }

    public String DepartmentLabel { get; set; }
    public String Department { get; set; }

    public String PictureUrl { get; set; }
    public String Email { get; set; }
    public Boolean Enabled { get; set; }
    public List<String> IdentityRoles { get; set; } = [];
}

public class UserRegisterDTO
{
    public required String FullName { get; set; }
    public required String Role { get; set; }
    public required String Department { get; set; }
    public String? PictureUrl { get; set; }
    public required String Email { get; set; }
    public required String Password { get; set; }
}
