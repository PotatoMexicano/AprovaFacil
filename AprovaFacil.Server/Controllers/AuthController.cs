
using AprovaFacil.Infra.Data.Identity;
using AprovaFacil.Server.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
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
            return Ok(new
            {
                Message = "Login bem-sucedido",
                User = new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                    user.Role,
                    user.Department,
                    user.PictureUrl,
                    user.Enabled,
                    IdentityRoles = roles
                }
            });
        }

        if (result.IsLockedOut)
        {
            return StatusCode(403, new { Message = "Conta bloqueada" });
        }

        return Unauthorized(new { Message = "Email ou senha inválidos" });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    [HttpGet("me")]
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
