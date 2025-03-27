namespace AprovaFacil.Domain.Models;

public class User
{
    public Int32 Id { get; set; }
    public String FullName { get; set; } = null!;
    public String Role { get; set; } = null!;
    public String Department { get; set; } = null!;
    public String PictureUrl { get; set; } = null!;

    public String Email { get; set; } = null!;
    public String Password { get; set; } = null!;

    public Boolean Enabled { get; set; }
}
