using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AprovaFacil.Server.Controllers;

[Route("api/user")]
[ApiController]
public class UserController(UserInterfaces.IUserService service) : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellation = default)
    {
        UserDTO[] result = await service.GetAllUsers(cancellation);
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
}

