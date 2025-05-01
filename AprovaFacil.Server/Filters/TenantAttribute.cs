using AprovaFacil.Server.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AprovaFacil.Server.Filters;

public class RequireTenantAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        System.Security.Claims.ClaimsPrincipal user = context.HttpContext.User;

        if (user.Identity is null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        Int32? tenantId = user.FindTenantId();

        if (tenantId == null)
        {
            context.Result = new ForbidResult();
        }
    }
}
