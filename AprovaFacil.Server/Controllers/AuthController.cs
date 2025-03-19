
using AprovaFacil.Application.Services;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Infra.Data.Identity;
using AprovaFacil.Server.DTOs;
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

            String token = _jwtService.GenerateJwtToken(user, roles);
            UserDTO userDTO = new UserDTO
            {
                Department = user.Department,
                Email = user.Email,
                Enabled = user.Enabled,
                FullName = user.FullName,
                Id = user.Id,
                PictureUrl = user.PictureUrl,
                Role = user.Role,
                IdentityRoles = [.. roles],
            };

            return Ok(new { Token = token, User = userDTO });
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
