using Enjaz.AI.Application.AI;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.AI.Endpoints.AI;

[ApiController]
[Route("api/v1/ai")]
public sealed class AiController(IIssueClassifier issueClassifier) : ControllerBase
{
    [HttpPost("classify-issue")]
    [Authorize]
    public async Task<IActionResult> ClassifyIssue(ClassifyIssueRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await issueClassifier.ClassifyAsync(request, cancellationToken));
    }

    private static IActionResult ToActionResult<T>(Result<T> result)
    {
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    }
}
