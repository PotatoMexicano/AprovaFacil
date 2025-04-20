using AprovaFacil.Domain.Constants;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Results;
using AprovaFacil.Server.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AprovaFacil.Server.Controllers;

[Route("api/user")]
[ApiController]
[Authorize]
public class UserController(UserInterfaces.IUserService service) : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellation = default)
    {
        Result<UserDTO[]> result = await service.GetAllUsers(cancellation);
        return result.ToActionResult();
    }

    [HttpGet("enabled")]
    public async Task<IActionResult> GetAllUsersEnabled(CancellationToken cancellation = default)
    {
        Result<UserDTO[]> result = await service.GetAllusersEnabled(cancellation);
        return result.ToActionResult();
    }

    [HttpGet("{idUser:int}")]
    public async Task<IActionResult> GetUser(Int32 idUser, CancellationToken cancellation = default)
    {
        Result<UserDTO> result = await service.GetUser(idUser, cancellation);
        return result.ToActionResult();
    }

    [HttpPost("{idUser:int}/disable")]
    [Authorize(Roles = $"{Roles.Manager}, {Roles.Director}")]
    public async Task<IActionResult> DisableUser(Int32 idUser, CancellationToken cancellation = default)
    {
        Result result = await service.DisableUser(idUser, cancellation);

        if (result.IsSuccess)
        {
            return StatusCode(StatusCodes.Status202Accepted);
        }
        else
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }
    }

    [HttpPost("{idUser:int}/enable")]
    [Authorize(Roles = $"{Roles.Manager}, {Roles.Director}")]
    public async Task<IActionResult> EnableUser(Int32 idUser, CancellationToken cancellation = default)
    {
        Result result = await service.EnableUser(idUser, cancellation);

        if (result.IsSuccess)
        {
            return StatusCode(StatusCodes.Status202Accepted);
        }
        else
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }
    }

    [HttpPost("register")]
    [Authorize(Roles = $"{Roles.Manager}, {Roles.Director}")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDTO request, CancellationToken cancellation = default)
    {
        Result<UserDTO> result = await service.RegisterUser(request, cancellation);
        return result.ToActionResult();
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO request, CancellationToken cancellation = default)
    {
        String? role = User.FindFirst(ClaimTypes.Role)?.Value;
        String? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        Boolean isAdmin = Roles.IsAdmin(role);
        Boolean isSame = String.Equals(request.Id.ToString(), userId, StringComparison.OrdinalIgnoreCase);

        if (!isAdmin && !isSame)
        {
            return Unauthorized(new ProblemDetails
            {
                Detail = "Usuário não autorizado",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        Result<UserDTO> result = await service.UpdateUser(request, cancellation);

        return result.ToActionResult();
    }
}

