using Microsoft.AspNetCore.Mvc;
using TmsApi.Application.DTOs;
using TmsApi.Services;

[ApiController]
[Route("api/students")]

public class StudetnsController(IStudentService studentService) : ControllerBase
{
    // GET/api/students returns all student records
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request, CancellationToken CancellationToken)
    {
        var students = await studentService.GetStudentsAsync(request, CancellationToken);
        return Ok(students);
    }

    // GET/api/students/{id} returns one or 404
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id,CancellationToken ct)
    {
        var student = await studentService.GetByIdAsync(id,ct);
        return student is not null ? Ok(student) : NotFound();
    }

    // POST /api/students creates and returns 201 with Location header
    [HttpPost]
    public async Task<IActionResult> Create(CreateStudentRequest request,CancellationToken ct)
    {
        if (await studentService.RegistrationNumberExistsAsync(request.RegistrationNumber, ct))
        {
            return Conflict(new ProblemDetails
            {
                Title = "Student registration number already exists",
                Detail = $"A student with the registration number '{request.RegistrationNumber}' already exists.",
                Status = StatusCodes.Status409Conflict,
            });
        }
        var student = await studentService.CreateAsync(request,ct);
        return CreatedAtAction(nameof(GetById), new { id = student?.Id }, student);
    }

    // DELETE /api/students/{id} returns 204 or 404
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await studentService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}