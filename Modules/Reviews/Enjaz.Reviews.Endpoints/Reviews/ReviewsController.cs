using Enjaz.Reviews.Application.Reviews;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Reviews.Endpoints.Reviews;

[ApiController]
public sealed class ReviewsController(IReviewsService reviewsService) : ControllerBase
{
    [Authorize(Roles = "Customer")]
    [HttpPost("api/v1/jobs/{jobId:guid}/review")]
    public async Task<IActionResult> Create(Guid jobId, CreateReviewRequest request, CancellationToken cancellationToken) => ToActionResult(await reviewsService.CreateForJobAsync(jobId, request, cancellationToken));

    [Authorize(Roles = "Customer,Technician")]
    [HttpGet("api/v1/jobs/{jobId:guid}/review")]
    public async Task<IActionResult> GetForJob(Guid jobId, CancellationToken cancellationToken) => ToActionResult(await reviewsService.GetForJobAsync(jobId, cancellationToken));

    [Authorize(Roles = "Technician")]
    [HttpGet("api/v1/technicians/reviews")]
    public async Task<IActionResult> GetMyTechnicianReviews(CancellationToken cancellationToken) => ToActionResult(await reviewsService.GetMyTechnicianReviewsAsync(cancellationToken));

    [Authorize(Roles = "Technician")]
    [HttpGet("api/v1/technicians/rating-summary")]
    public async Task<IActionResult> GetMyRatingSummary(CancellationToken cancellationToken) => ToActionResult(await reviewsService.GetMyTechnicianRatingSummaryAsync(cancellationToken));

    [AllowAnonymous]
    [HttpGet("api/v1/reviews/technicians/{technicianId:guid}")]
    public async Task<IActionResult> GetTechnicianReviews(Guid technicianId, CancellationToken cancellationToken) => ToActionResult(await reviewsService.GetTechnicianReviewsAsync(technicianId, cancellationToken));

    [Authorize(Roles = "Admin")]
    [HttpGet("api/v1/admin/reviews")]
    public async Task<IActionResult> GetReviews([FromQuery] Guid? technicianId, [FromQuery] Guid? jobId, [FromQuery] bool? isVisible, CancellationToken cancellationToken) => ToActionResult(await reviewsService.GetReviewsAsync(new ReviewQuery(technicianId, jobId, isVisible), cancellationToken));

    [Authorize(Roles = "Admin")]
    [HttpPost("api/v1/admin/reviews/{id:guid}/hide")]
    public async Task<IActionResult> Hide(Guid id, CancellationToken cancellationToken) => ToActionResult(await reviewsService.SetVisibilityAsync(id, false, cancellationToken));

    [Authorize(Roles = "Admin")]
    [HttpPost("api/v1/admin/reviews/{id:guid}/show")]
    public async Task<IActionResult> Show(Guid id, CancellationToken cancellationToken) => ToActionResult(await reviewsService.SetVisibilityAsync(id, true, cancellationToken));

    private static IActionResult ToActionResult<T>(Result<T> result) => result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
}
