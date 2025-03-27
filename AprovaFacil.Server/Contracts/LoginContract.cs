namespace AprovaFacil.Server.Contracts;

public class LoginContract
{
    public required String Email { get; set; } = null!;
    public required String Password { get; set; } = null!;
}
