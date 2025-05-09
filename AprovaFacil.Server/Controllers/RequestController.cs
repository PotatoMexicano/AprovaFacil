using AprovaFacil.Domain.Constants;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Filters;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Results;
using AprovaFacil.Server.Contracts;
using AprovaFacil.Server.Extensions;
using AprovaFacil.Server.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AprovaFacil.Server.Controllers;

[Route("api/request")]
[ApiController]
[RequireTenant]
[Authorize]
public class RequestController(RequestInterfaces.IRequestService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = $"{Roles.Manager}, {Roles.Director}")]
    public async Task<IActionResult> ListAllRequests(CancellationToken cancellation = default)
    {
        Result<RequestDTO[]> requests = await service.ListAll(cancellation);
        return requests.ToActionResult();
    }

    [HttpGet("{uuidRequest}")]
    public async Task<IActionResult> ListRequest(String uuidRequest, CancellationToken cancellation = default)
    {
        Int32? userId = User.FindUserIdentifier();
        String? role = User.FindFirst(ClaimTypes.Role)?.Value;

        Boolean isAdmin = Roles.IsAdmin(role);
        Boolean isFinance = Roles.IsFinance(role);

        if (!Guid.TryParse(uuidRequest, out Guid requestGuid))
        {
            return BadRequest(new ProblemDetails
            {
                Detail = "UUID not valid."
            });
        }

        Result<RequestDTO>? result = await service.ListRequest(requestGuid, cancellation);

        if (result.IsFailure)
        {
            return result.ToActionResult();
        }

        if (result.Value is null)
        {
            return NotFound(new ProblemDetails
            {
                Detail = "Nenhuma solicitação encontrada."
            });
        }

        Boolean isOwner = result.Value.RequesterId == userId;

        if (!isAdmin && !isOwner && !isFinance)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "You cannot access this request.",
                Detail = "You don't have permission to see this request.",
                Status = StatusCodes.Status401Unauthorized,
            });
        }

        return result.ToActionResult();
    }

    [HttpPost("myself")]
    public async Task<IActionResult> ListUserRequests([FromBody] FilterRequest request, CancellationToken cancellation = default)
    {
        Int32? userId = User.FindUserIdentifier();

        if (userId is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Detail = "Are you logged in ?"
            });
        }

        Result<RequestDTO[]> result = await service.ListUserRequests(request, userId.Value, cancellation);

        return result.ToActionResult();
    }

    [HttpPost("tenant")]
    public async Task<IActionResult> ListTenantRequests([FromBody] FilterRequest request, CancellationToken cancellation = default)
    {
        Int32? userId = User.FindUserIdentifier();

        if (userId is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Detail = "Are you logged in ?"
            });
        }

        Result<RequestDTO[]> result = await service.ListTenantRequests(request, cancellation);

        return result.ToActionResult();
    }

    [Authorize]
    [HttpGet("myself/stats")]
    public async Task<IActionResult> UserStats(CancellationToken cancellation = default)
    {
        Int32? userId = User.FindUserIdentifier();

        if (userId is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Detail = "Are you logged in ?"
            });
        }

        Result<Object> result = await service.UserStats(userId.Value, cancellation);

        return result.ToActionResult();
    }

    [Authorize(Roles = $"{Roles.Director}, {Roles.Manager}")]
    [HttpGet("tenant/stats")]
    public async Task<IActionResult> TenantStats(CancellationToken cancellation = default)
    {
        Int32? userId = User.FindUserIdentifier();

        if (userId is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Detail = "Are you logged in ?"
            });
        }

        Result<Object> result = await service.TenantStats(cancellation);

        return result.ToActionResult();
    }

    [HttpPost("pending")]
    [Authorize(Roles = $"{Roles.Manager}, {Roles.Director}")]
    public async Task<IActionResult> ListPendingRequst(CancellationToken cancellation = default)
    {
        FilterRequest request = new FilterRequest();

        Int32? userId = User.FindUserIdentifier();
        String? userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (userId is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Detail = "Are you logged in ?"
            });
        }

        if (userRole == Roles.Director)
        {
            request.UserRole = Roles.Director;
            request.ApplicationUserId = userId;
        }
        else if (userRole == Roles.Manager)
        {
            request.UserRole = Roles.Manager;
            request.ApplicationUserId = userId;
        }

        Result<RequestDTO[]> result = await service.ListPendingRequests(request, cancellation);

        return result.ToActionResult();
    }

    [HttpPost("approved")]
    [Authorize(Roles = $"{Roles.Finance}")]
    public async Task<IActionResult> ListApprovedRequest(CancellationToken cancellation = default)
    {
        FilterRequest request = new FilterRequest();

        Int32? userId = User.FindUserIdentifier();
        String? userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (userId is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Detail = "Are you logged in ?"
            });
        }

        request.UserRole = userRole;
        request.ApplicationUserId = userId;

        Result<RequestDTO[]> result = await service.ListApprovedRequests(request, cancellation);

        return result.ToActionResult();
    }

    [HttpPost("finished")]
    [Authorize(Roles = $"{Roles.Finance}, {Roles.Director}, {Roles.Manager}")]
    public async Task<IActionResult> ListFinishedRequest(CancellationToken cancellation = default)
    {
        FilterRequest request = new FilterRequest();

        Int32? userId = User.FindUserIdentifier();
        String? userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (userId is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Detail = "Are you logged in ?"
            });
        }

        request.UserRole = userRole;
        request.ApplicationUserId = userId;

        Result<RequestDTO[]> result = await service.ListFinishedRequests(request, cancellation);

        return result.ToActionResult();
    }

    [HttpGet("file/{type}/{uuidRequest}/{uuidFile}")]
    public async Task<IActionResult> LoadFileRequest(String type, String uuidRequest, String uuidFile, CancellationToken cancellation = default)
    {
        if (!String.Equals(type, "invoice", StringComparison.InvariantCultureIgnoreCase) && !String.Equals(type, "budget", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new ProblemDetails
            {
                Detail = "type not recogned."
            });
        }

        if (!Guid.TryParse(uuidRequest, out Guid guidRequest))
        {
            return BadRequest(new ProblemDetails
            {
                Detail = "RequestUUID is not valid."
            });
        }

        if (!Guid.TryParse(uuidFile, out Guid guidFile))
        {
            return BadRequest(new ProblemDetails
            {
                Detail = "FileUUID is not valid."
            });
        }

        cancellation.ThrowIfCancellationRequested();

        Result<Byte[]> response = await service.LoadFileRequest(type, guidRequest, guidFile, cancellation);

        if (response.IsFailure)
        {
            return response.ToActionResult();
        }

        if (response.Value is not null && response.Value.Length > 0)
        {
            String fileName = guidFile + ".pdf";

            Response.Headers["Content-type"] = "application/pdf";
            Response.Headers["Content-Disposition"] = $"attachment; filename={fileName}";

            return File(response.Value, "application/pdf", fileName);
        }

        return NotFound(new ProblemDetails
        {
            Detail = "Arquivo não encontrado."
        });

    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterRequest([FromForm] HttpRegisterRequest request, CancellationToken cancellation = default)
    {
        if (request.Invoice != null)
        {
            if (request.Invoice.Length > 20 * 1024 * 1024)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invoice too large",
                    Detail = "The invoice file is too large. The maximum size is 20MB.",
                    Status = StatusCodes.Status413PayloadTooLarge
                });
            }

            String[] allowedTypes = new[] { "application/pdf" };
            if (!allowedTypes.Contains(request.Invoice.ContentType))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid invoice type",
                    Detail = "The invoice file must be a PDF.",
                    Status = StatusCodes.Status415UnsupportedMediaType
                });
            }
        }

        if (request.Budget != null)
        {
            if (request.Budget.Length > 20 * 1024 * 1024)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Budget too large",
                    Detail = "The budget file is too large. The maximum size is 20MB.",
                    Status = StatusCodes.Status413PayloadTooLarge
                });
            }
            String[] allowedTypes = new[] { "application/pdf" };
            if (!allowedTypes.Contains(request.Budget.ContentType))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid budget type",
                    Detail = "The budget file must be a PDF.",
                    Status = StatusCodes.Status415UnsupportedMediaType
                });
            }
        }

        Byte[]? invoiceBytes = null;
        Byte[]? budgetBytes = null;

        cancellation.ThrowIfCancellationRequested();

        if (request.Invoice != null)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                await request.Invoice.CopyToAsync(stream, cancellation);
                invoiceBytes = stream.ToArray();
            }
        }

        if (request.Budget != null)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                await request.Budget.CopyToAsync(stream, cancellation);
                budgetBytes = stream.ToArray();
            }
        }

        Int32? userId = User.FindUserIdentifier();

        if (userId is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Detail = "Are you logged in ?"
            });
        }

        Result<RequestDTO> result = await service.RegisterRequest(new RequestRegisterDTO
        {
            Budget = budgetBytes ?? [],
            Invoice = invoiceBytes ?? [],
            Amount = request.Amount,
            CompanyId = request.CompanyId,
            Note = request.Note,
            RequesterId = userId.Value,
            DirectorsIds = request.DirectorsIds,
            ManagersId = request.ManagersId,
            PaymentDate = request.PaymentDate,
            InvoiceFileName = request.Invoice?.FileName,
            BudgetFileName = request.Budget?.FileName
        }, cancellation);

        if (result.IsFailure)
        {
            return result.ToActionResult();
        }

        return result.ToActionResult();
    }

    [HttpPost("{uuidRequest}/approve")]
    [Authorize(Roles = $"{Roles.Manager}, {Roles.Director}")]
    public async Task<IActionResult> ApproveRequest(String uuidRequest, CancellationToken cancellation = default)
    {
        // Receber o id da solicitação, id do usuario.
        Int32? userId = User.FindUserIdentifier();

        if (userId is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Detail = "Are you logged in ?"
            });
        }

        if (!Guid.TryParse(uuidRequest, out Guid requestGuid))
        {
            return BadRequest(new ProblemDetails
            {
                Detail = "UUID not valid."
            });
        }

        await service.ApproveRequest(requestGuid, userId.Value, cancellation);

        return NoContent();
    }

    [HttpPost("{uuidRequest}/reject")]
    [Authorize(Roles = $"{Roles.Manager}, {Roles.Director}")]
    public async Task<IActionResult> RejectRequest(String uuidRequest, CancellationToken cancellation = default)
    {
        // Receber o id da solicitação, id do usuario.
        Int32? userId = User.FindUserIdentifier();

        if (userId is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Detail = "Are you logged in ?"
            });
        }

        if (!Guid.TryParse(uuidRequest, out Guid requestGuid))
        {
            return BadRequest(new ProblemDetails
            {
                Detail = "UUID not valid."
            });
        }

        await service.RejectRequest(requestGuid, userId.Value, cancellation);

        return NoContent();
    }

    [HttpPost("{uuidRequest}/finish")]
    public async Task<IActionResult> FinishRequest(String uuidRequest, CancellationToken cancellation = default)
    {
        // Receber o id da solicitação, id do usuario.
        Int32? userId = User.FindUserIdentifier();

        if (userId is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Detail = "Are you logged in ?"
            });
        }

        if (!Guid.TryParse(uuidRequest, out Guid requestGuid))
        {
            return BadRequest(new ProblemDetails
            {
                Detail = "UUID not valid."
            });
        }

        await service.FinishRequest(requestGuid, userId.Value, cancellation);

        return NoContent();
    }
}
