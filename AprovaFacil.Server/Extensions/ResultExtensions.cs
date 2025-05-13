using AprovaFacil.Domain.Results;
using Microsoft.AspNetCore.Mvc;

namespace AprovaFacil.Server.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess) return new OkObjectResult(result.Value);

        if (result.IsFailure && result.Error is not null)
        {
            return result.Error.Type switch
            {
                ErrorType.NotFound => new NotFoundObjectResult(new ProblemDetails { Detail = result.Error.Message }),
                ErrorType.Validation => new BadRequestObjectResult(new ProblemDetails { Detail = result.Error.Message }),
                ErrorType.Conflict => new ConflictObjectResult(new ProblemDetails { Detail = result.Error.Message }),
                ErrorType.Unathorized => new UnauthorizedObjectResult(new ProblemDetails { Detail = result.Error.Message }),
                ErrorType.InternalError => new ObjectResult(new ProblemDetails { Status = 500, Detail = result.Error.Message }),
                ErrorType.Forbidden => new UnauthorizedObjectResult(new ProblemDetails { Status = 403, Detail = result.Error.Message }),
                _ => new BadRequestObjectResult(new ProblemDetails { Detail = result.Error.Message }),
            };
        }

        return new BadRequestObjectResult(new ProblemDetails { Detail = "Erro genérico." });
    }

    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess) return new OkObjectResult(result);

        if (result.IsFailure && result.Error is not null)
        {
            return result.Error.Type switch
            {
                ErrorType.NotFound => new NotFoundObjectResult(new ProblemDetails { Detail = result.Error.Message }),
                ErrorType.Validation => new BadRequestObjectResult(new ProblemDetails { Detail = result.Error.Message }),
                ErrorType.Conflict => new ConflictObjectResult(new ProblemDetails { Detail = result.Error.Message }),
                ErrorType.Unathorized => new UnauthorizedObjectResult(new ProblemDetails { Detail = result.Error.Message }),
                ErrorType.InternalError => new ObjectResult(new ProblemDetails { Status = 500, Detail = result.Error.Message }),
                ErrorType.Forbidden => new UnauthorizedObjectResult(new ProblemDetails { Status = 403, Detail = result.Error.Message }),
                _ => new BadRequestObjectResult(new ProblemDetails { Detail = result.Error.Message }),
            };
        }

        return new BadRequestObjectResult(new ProblemDetails { Detail = "Erro genérico." });
    }
}
