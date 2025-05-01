using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Results;
using AprovaFacil.Server.Extensions;
using AprovaFacil.Server.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AprovaFacil.Server.Controllers;

[Route("api/company")]
[ApiController]
[RequireTenant]
[Authorize]
public class CompanyController(CompanyInterfaces.ICompanyService companyService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllCompanies(CancellationToken cancellation = default)
    {
        Result<CompanyDTO[]> companies = await companyService.GetAllCompanies(cancellation);
        return companies.ToActionResult();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCompany(Int32 id, CancellationToken cancellation = default)
    {
        Result<CompanyDTO>? company = await companyService.GetCompany(id, cancellation);
        return company.ToActionResult();
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateCompany([FromBody] CompanyDTO request, CancellationToken cancellation = default)
    {
        Result<CompanyDTO> company = await companyService.UpdateCompany(request, cancellation);
        return company.ToActionResult();
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterCompany([FromBody] CompanyDTO request, CancellationToken cancellation = default)
    {
        Result<CompanyDTO> company = await companyService.RegisterCompany(request, cancellation);

        if (company.IsFailure && company.Error is not null) return BadRequest(new ProblemDetails { Detail = company.Error.Message });

        return StatusCode(StatusCodes.Status201Created, company.Value);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCompany(Int32 id, CancellationToken cancellation = default)
    {
        await companyService.DeleteCompany(id, cancellation);
        return StatusCode(StatusCodes.Status202Accepted);
    }
}