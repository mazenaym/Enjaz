using Enjaz.SharedKernel.Results;
using Enjaz.Technicians.Application.Technicians;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Technicians.Endpoints.Technicians;

[ApiController]
[Route("api/v1/technicians")]
public sealed class TechniciansController(ITechnicianService technicianService) : ControllerBase
{
    [HttpPost("apply")]
    [Authorize]
    public async Task<IActionResult> Apply(ApplyAsTechnicianRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await technicianService.ApplyAsync(request, cancellationToken));
    }

    [HttpGet("me")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        return ToActionResult(await technicianService.GetMyProfileAsync(cancellationToken));
    }

    [HttpPut("me")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> UpdateMe(UpdateTechnicianProfileRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await technicianService.UpdateMyProfileAsync(request, cancellationToken));
    }

    [HttpPost("documents")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> AddDocument(TechnicianDocumentRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await technicianService.AddDocumentAsync(request, cancellationToken));
    }

    [HttpGet("documents")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> GetDocuments(CancellationToken cancellationToken)
    {
        return ToActionResult(await technicianService.GetMyDocumentsAsync(cancellationToken));
    }

    [HttpPut("skills")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> UpdateSkills(UpdateTechnicianSkillsRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await technicianService.UpdateSkillsAsync(request, cancellationToken));
    }

    [HttpPost("go-online")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> GoOnline(CancellationToken cancellationToken)
    {
        return ToActionResult(await technicianService.GoOnlineAsync(cancellationToken));
    }

    [HttpPost("go-offline")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> GoOffline(CancellationToken cancellationToken)
    {
        return ToActionResult(await technicianService.GoOfflineAsync(cancellationToken));
    }

    private static IActionResult ToActionResult(Result result)
    {
        return result.IsSuccess
            ? new OkObjectResult(new { message = "Success" })
            : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    }

    private static IActionResult ToActionResult<T>(Result<T> result)
    {
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    }
}
