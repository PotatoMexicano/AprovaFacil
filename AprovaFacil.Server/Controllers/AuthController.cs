
using AprovaFacil.Application.Services;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Extensions;
using AprovaFacil.Infra.Data.Identity;
using AprovaFacil.Server.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AprovaFacil.Server.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtService _jwtService;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, JwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginContract request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);
            IList<String> roles = await _userManager.GetRolesAsync(user);
            IList<System.Security.Claims.Claim> claims = await _userManager.GetClaimsAsync(user);

            String token = _jwtService.GenerateJwtToken(user, roles);

            UserDTO userDTO = user.ToDTO(roles);

            return Ok(new { Token = token, User = userDTO });
        }

        if (result.IsLockedOut)
        {
            return StatusCode(403, new { Message = "Conta bloqueada" });
        }

        return Unauthorized(new { Message = "Email ou senha inválidos" });
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
            IdentityRoles = roles
        });
    }
}
