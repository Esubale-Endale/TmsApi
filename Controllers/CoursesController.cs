using Microsoft.AspNetCore.Mvc;
using Tms.Api.Dtos;
using Tms.Api.Entities;
using Tms.Api.Services;

[ApiController]
[Route("api/courses")]
public class CoursesController(ICourseService courseService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request, CancellationToken ct)
    {
        var result = await courseService.GetCoursesAsync(request, ct);
        return Ok(result);
    }
    [HttpGet("{id:int}", Name = nameof(GetCourseById))]
    public async Task<IActionResult> GetCourseById(int id, CancellationToken ct)
    {
        var course = await courseService.GetByIdAsync(id, ct);
        return course is not null ? Ok(course) : NotFound();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateCourseRequest request, CancellationToken ct)
    {
        if (await courseService.CodeExistsAsync(request.Code, ct))
        {
            return Conflict(new ProblemDetails
            {
                Title = "Course code already exists",
                Detail = $"A course with the code '{request.Code}' already Registered.",
                Status = StatusCodes.Status409Conflict,
            });
        }
        var result = await courseService.CreateAsync(request, ct);

        return CreatedAtAction(
            nameof(GetCourseById),
            new { id = result.Id },
            result);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await courseService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}