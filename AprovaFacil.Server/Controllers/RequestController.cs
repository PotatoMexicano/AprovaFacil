using AprovaFacil.Domain.DTOs;
using AprovaFacil.Server.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AprovaFacil.Server.Controllers;

[Route("api/request")]
[ApiController]
public class RequestController : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> RegisterRequest([FromForm] HttpRequestDTO request, CancellationToken cancellation = default)
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

        RequestDTO requestDTO = new RequestDTO
        {
            ManagerId = request.ManagerId,
            DirectorsIds = request.DirectorsIds,
            CompanyId = request.CompanyId,
            PaymentDate = request.PaymentDate,
            Amount = request.Amount,
            Note = request.Note,
            Invoice = invoiceBytes,
            Budget = budgetBytes
        };

        return Ok(requestDTO);
    }
}
