namespace AprovaFacil.Domain.DTOs;

public class UserDTO
{
    public Int32 Id { get; set; }
    public required String FullName { get; set; }

    public virtual String? RoleLabel { get; set; }
    public required String Role { get; set; }

    public virtual String? DepartmentLabel { get; set; }
    public required String Department { get; set; }

    public String? PictureUrl { get; set; }
    public required String Email { get; set; }
    public Boolean Enabled { get; set; }
    public List<String> IdentityRoles { get; set; } = [];

    public String TenantName { get; set; } = null!;
    public Int32 TenantId { get; set; }

    public Int32 RequestApproved { get; set; }
}

public class UserRegisterDTO
{
    public required String FullName { get; set; }
    public required String Role { get; set; }
    public required String Department { get; set; }
    public String? PictureUrl { get; set; }
    public required String Email { get; set; }
    public required String Password { get; set; }
    public Int32 TenantId { get; set; }
}

public class UserUpdateDTO
{
    public required Int32 Id { get; set; }
    public required String FullName { get; set; }
    public required String Email { get; set; }
    public String? Password { get; set; }
    public required String Role { get; set; }
    public required String Department { get; set; }
    public required String PictureUrl { get; set; }
}