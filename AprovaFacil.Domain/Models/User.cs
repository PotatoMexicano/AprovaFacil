namespace AprovaFacil.Domain.Models;

public class User
{
    public Int32 Id { get; set; }
    public String FullName { get; set; }
    public String Role { get; set; }
    public String Department { get; set; }
    public String PictureUrl { get; set; }

    public String Email { get; set; }
    public String Password { get; set; }

    public Boolean Enabled { get; set; }
}
