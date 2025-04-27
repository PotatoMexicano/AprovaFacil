using System.Security.Claims;

namespace AprovaFacil.Server.Extensions;

public static class ClaimsExtensions
{
    public static Int32? FindUserIdentifier(this ClaimsPrincipal principal)
    {
        String? userIdentifier = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdentifier is null)
        {
            return null;
        }

        if (!Int32.TryParse(userIdentifier, out Int32 userId))
        {
            return null;
        }

        return userId;

    }
}
