namespace AprovaFacil.Domain.Constants;

public static class Roles
{
    public const String Requester = nameof(Requester);
    public const String Manager = nameof(Manager);
    public const String Director = nameof(Director);
    public const String Finance = nameof(Finance);
    public const String Assistant = nameof(Assistant);

    public static Boolean IsFinance(String? role)
    {
        if (String.IsNullOrEmpty(role)) return false;
        if (role.Equals(Finance, StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }

    public static Boolean IsAdmin(String? role)
    {
        if (String.IsNullOrEmpty(role)) return false;
        if (role.Equals(Manager, StringComparison.OrdinalIgnoreCase)) return true;
        if (role.Equals(Director, StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }
}
