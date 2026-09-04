using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Application.Courses.Commands.CreateCourse;
using TmsApi.Application.DTOs;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Api.Controllers.V2;

[Authorize(Roles = "Admin,Instructor,Student")]
[ApiController]
[ApiExplorerSettings(GroupName = "v2")]
[Route("api/V{version:apiVersion}/courses")]
[ApiVersion("2.0")]
public class CourseController(TmsDbContext context, IMediator mediator, IAuthorizationService authorizationService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CourseResponseDto>), StatusCodes.Status200OK)]
    [EndpointSummary("List courses with pagination")]
    [EndpointDescription("Returns a paginated, optionally filtered list of TMS courses. PageSize is capped at 50.")]
    public async Task<IActionResult> GetCourses([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var baseQuery = context.Courses.AsNoTracking();
        var totalCount = await baseQuery.CountAsync(cancellationToken: ct);
        var rows = await baseQuery
            .OrderBy(c => c.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                c.Id,
                c.Title,
                c.Code,
                c.MaxCapacity,
                EnrollmentCount = c.Enrollments.Count
            })
            .ToListAsync(ct);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var hasNext = page < totalPages;
        var hasPrevious = page > 1;

        return Ok(new
        {
            data = rows,
            meta = new
            {
                totalCount,
                page,
                pageSize,
                totalPages,
                hasNext,
                hasPrevious
            },
            links = new
            {
                self = $"/api/v2/courses?page={page}&pageSize={pageSize}",
                next = hasNext ? $"/api/v2/courses?page={page + 1}&pageSize={pageSize}" : null,
                prev = hasPrevious ? $"/api/v2/courses?page={page - 1}&pageSize={pageSize}" : null,
                enroll = "/api/v2/enrollments"
            }
        });
    }

    [HttpPost]
    [ProducesResponseType(typeof(CourseResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [EndpointSummary("Create a new course")]
    [EndpointDescription("Creates a course with a unique code. Returns409 if the course code already exists.")]
    public async Task<IActionResult> Create(CreateCourseCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.Match<IActionResult>(
            onSuccess: created => CreatedAtAction(nameof(GetCourses), new { id = created.CourseId }, created),
            onFailure: error =>
            {
                var status = error.Code switch
                {
                    "already_created" => StatusCodes.Status409Conflict,
                    _ => StatusCodes.Status400BadRequest
                };
                return Problem(
                    statusCode: status,
                    title: "Course creation failed",
                    detail: error.Message,
                    type: $"https://tms.local/errors/{error.Code}");
            });
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseDto dto)
    {
        var course = await context.Courses.FindAsync(id);
        if (course == null) return NotFound();
        var authResult = await
        authorizationService.AuthorizeAsync(User, course, "CanEditCourse");
        if (!authResult.Succeeded)
        {
            return Forbid(); // 403 Forbidden when caller doesn't own the resource
        }
        course.Title = dto.Title;
        await context.SaveChangesAsync();
        return NoContent();
    }

    // [HttpGet("search")]
    // [EnableRateLimiting("search")]
    // public async Task<IActionResult> SearchCourses(
    // [FromQuery] string? term, CancellationToken ct)
    // {
    //     var results = await mediator.Send(new SearchCoursesQuery(term), ct);
    //     return Ok(results);
    // }

}