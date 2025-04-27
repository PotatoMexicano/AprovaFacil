namespace AprovaFacil.Domain.Constants;

public static class LevelRequest
{
    public const Int32 Pending = 0; // Registered
    public const Int32 FirstLevel = 1; // Approved by manager
    public const Int32 SecondLevel = 2; // Approved by director
    public const Int32 Finished = 3; // Finished by finance
}
