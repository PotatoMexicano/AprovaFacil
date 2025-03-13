using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AprovaFacil.Server.Controllers;

[Route("api/company")]
[ApiController]
public class CompanyController(CompanyInterfaces.ICompanyService companyService) : ControllerBase
{
    [HttpGet("")]
    public async Task<ActionResult<CompanyDTO[]>> GetAllCompanies(CancellationToken cancellation = default)
    {
        CompanyDTO[] companies = await companyService.GetAllCompanies(cancellation);
        return Ok(companies);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CompanyDTO>> GetCompany(Int32 id, CancellationToken cancellation = default)
    {
        CompanyDTO? company = await companyService.GetCompany(id, cancellation);
        return company is null ? NotFound() : Ok(company);
    }

    [HttpPost("update")]
    public async Task<ActionResult> UpdateCompany([FromBody] CompanyDTO request, CancellationToken cancellation = default)
    {
        try
        {
            CompanyDTO? company = await companyService.UpdateCompany(request, cancellation);
            return StatusCode(StatusCodes.Status202Accepted, company);
        }
        catch (Exception ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Company not updated",
                Status = StatusCodes.Status304NotModified,
                Detail = ex.Message
            });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult> RegisterCompany([FromBody] CompanyDTO request, CancellationToken cancellation = default)
    {
        try
        {
            CompanyDTO? company = await companyService.RegisterCompany(request, cancellation);
            return StatusCode(StatusCodes.Status201Created, company);
        }
        catch (Exception ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Company not registered",
                Status = StatusCodes.Status406NotAcceptable,
                Detail = ex.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteCompany(Int32 id, CancellationToken cancellation = default)
    {
        try
        {
            await companyService.DeleteCompany(id, cancellation);
            return StatusCode(StatusCodes.Status202Accepted);
        }
        catch (Exception ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Company not removed",
                Detail = ex.Message,
                Status = StatusCodes.Status304NotModified
            });
        }
    }
}