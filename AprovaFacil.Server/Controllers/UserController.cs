using AprovaFacil.Domain.Constants;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Interfaces;
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
        UserDTO[] result = await service.GetAllUsers(cancellation);
        return Ok(result);
    }

    [HttpGet("enabled")]
    public async Task<IActionResult> GetAllUsersEnabled(CancellationToken cancellation = default)
    {
        UserDTO[] result = await service.GetAllusersEnabled(cancellation);
        return Ok(result);
    }

    [HttpGet("{idUser:int}")]
    public async Task<IActionResult> GetUser(Int32 idUser, CancellationToken cancellation = default)
    {
        UserDTO? result = await service.GetUser(idUser, cancellation);

        if (result is null)
        {
            return NotFound(new ProblemDetails
            {
                Detail = $"Usuário com ID {idUser} não encontrado.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(result);
    }

    [HttpPost("{idUser:int}/disable")]
    [Authorize(Roles = $"{Roles.Manager}, {Roles.Director}")]
    public async Task<IActionResult> DisableUser(Int32 idUser, CancellationToken cancellation = default)
    {
        Boolean result = await service.DisableUser(idUser, cancellation);

        if (result)
        {
            return StatusCode(StatusCodes.Status202Accepted, result);
        }
        else
        {
            return StatusCode(StatusCodes.Status304NotModified, result);
        }
    }

    [HttpPost("{idUser:int}/enable")]
    [Authorize(Roles = $"{Roles.Manager}, {Roles.Director}")]
    public async Task<IActionResult> EnableUser(Int32 idUser, CancellationToken cancellation = default)
    {
        Boolean result = await service.EnableUser(idUser, cancellation);

        if (result)
        {
            return StatusCode(StatusCodes.Status202Accepted, result);
        }
        else
        {
            return StatusCode(StatusCodes.Status304NotModified, result);
        }
    }

    [HttpPost("register")]
    [Authorize(Roles = $"{Roles.Manager}, {Roles.Director}")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDTO request, CancellationToken cancellation = default)
    {
        UserDTO? result = await service.RegisterUser(request, cancellation);

        if (result is null)
        {
            return BadRequest(new ProblemDetails
            {
                Detail = "Não foi possível registrar o usuário.",
                Status = StatusCodes.Status400BadRequest
            });

        }

        return Ok(result);
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

        UserDTO? result = await service.UpdateUser(request, cancellation);

        if (result is null)
        {
            return BadRequest(new ProblemDetails
            {
                Detail = "Falha ao atualizar usuário"
            });
        }

        return Ok(result);
    }
}

