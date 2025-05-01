using AprovaFacil.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AprovaFacil.Application.Services;

public class TenantProvider(IHttpContextAccessor httpContextAccessor) : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Int32? GetTenantId()
    {
        System.Security.Claims.ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;

        if (user is null)
        {
            return null;
        }

        if (user.Identity is null)
        {
            return null;
        }

        if (!user.Identity.IsAuthenticated)
        {
            return null;
        }

        String? tenant = user.FindFirst("TenantId")?.Value;

        if (Int32.TryParse(tenant, out Int32 tenantId))
        {
            return tenantId;
        }

        return null;
    }
}
