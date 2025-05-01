using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Extensions;
using AprovaFacil.Infra.Data.Identity;
using AprovaFacil.Server.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AprovaFacil.Server.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginContract request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Unauthorized();
        }

        if (!user.Enabled)
        {
            return StatusCode(403, new { Message = "Conta bloqueada" });
        }

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { Message = "Email ou senha inválidos" });
        }

        IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
        userClaims.Add(new Claim("TenantId", user.TenantId.ToString()));

        await _signInManager.SignInWithClaimsAsync(user, isPersistent: true, userClaims);

        IList<String> roles = await _userManager.GetRolesAsync(user);

        UserDTO userDTO = user.ToDTO(roles);

        return Ok(new { User = userDTO });

    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> CurrentUser()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "User unauthorized",
            });
        }

        ApplicationUser? user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = "User not found"
            });
        }

        IList<String> roles = await _userManager.GetRolesAsync(user);

        Response.Headers.CacheControl = "no-store, no-cache, must-revalidade, max-age=0";
        Response.Headers.Pragma = "no-cache";

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.Role,
            user.Department,
            user.PictureUrl,
            user.Enabled,
            user.Tenant, // PRECISAR DO TENANT_NAME
            IdentityRoles = roles
        });
    }
}
