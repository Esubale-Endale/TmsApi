using Microsoft.AspNetCore.Mvc;
using TmsApi.Dtos;
using TmsApi.Entities;
using TmsApi.Services;

[ApiController]
[Route("api/courses")]
public class CoursesController(ICourseService courseService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var courses = await courseService.GetAllAsync(ct);
        return Ok(courses);
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
        var newCourse = await courseService.CreateAsync(request, ct);

        return CreatedAtAction(
            nameof(GetCourseById),
            new { id = newCourse.Id },
            newCourse);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await courseService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}