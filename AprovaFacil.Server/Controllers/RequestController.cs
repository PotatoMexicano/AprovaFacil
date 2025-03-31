using AprovaFacil.Domain.Constants;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Filters;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Server.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AprovaFacil.Server.Controllers;

[Route("api/request")]
[ApiController]
[Authorize]
public class RequestController(RequestInterfaces.IRequestService service) : ControllerBase
{
    [HttpPost("myself")]
    public async Task<IActionResult> ListRequest([FromBody] FilterRequest request, CancellationToken cancellation = default)
    {
        String? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Detail = "Are you logged in ?"
            });
        }

        RequestDTO[] result = await service.ListRequests(request, userId, cancellation);

        return Ok(result);
    }

    [HttpPost("pending")]
    [Authorize(Roles = $"{Roles.Manager}, {Roles.Director}")]
    public async Task<IActionResult> ListPendingRequst([FromBody] FilterRequest request, CancellationToken cancellation = default)
    {
        String? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Detail = "Are you logged in ?"
            });
        }

        RequestDTO[] result = await service.ListPendingRequests(request, cancellation);

        return Ok(result);
    }

    [HttpGet("file/{type}/{uuidRequest}/{uuidFile}")]
    public async Task<IActionResult> LoadFileRequest(String type, String uuidRequest, String uuidFile, CancellationToken cancellation = default)
    {
        if (
            !String.Equals(type, "invoice", StringComparison.InvariantCultureIgnoreCase) &&
            !String.Equals(type, "budget", StringComparison.OrdinalIgnoreCase))
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

        Byte[] response = await service.LoadFileRequest(type, guidRequest, guidFile, cancellation);

        if (response.Length > 0)
        {

            String fileName = guidFile + ".pdf";

            Response.Headers["Content-type"] = "application/pdf";
            Response.Headers["Content-Disposition"] = $"attachment; filename={fileName}";

            return File(response, "application/pdf", fileName);

        }

        return NotFound(new ProblemDetails
        {
            Detail = "File not found."
        });

    }

    [HttpPost("register")]
    public async Task<ActionResult<RequestDTO>> RegisterRequest([FromForm] HttpRegisterRequest request, CancellationToken cancellation = default)
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

        if (!Int32.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Int32 idUserAuthenticated))
        {
            return BadRequest(new ProblemDetails
            {
                Detail = "Are you logged ?",
                Status = StatusCodes.Status400BadRequest
            });
        }

        RequestDTO result = await service.RegisterRequest(new RequestRegisterDTO
        {
            Budget = budgetBytes ?? [],
            Invoice = invoiceBytes ?? [],
            Amount = request.Amount,
            CompanyId = request.CompanyId,
            Note = request.Note,
            RequesterId = idUserAuthenticated,
            DirectorsIds = request.DirectorsIds,
            ManagersId = request.ManagersId,
            PaymentDate = request.PaymentDate,
            InvoiceFileName = request.Invoice?.FileName,
            BudgetFileName = request.Budget?.FileName
        }, cancellation);

        return Ok(result);
    }

}
