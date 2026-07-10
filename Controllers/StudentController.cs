using Microsoft.AspNetCore.Mvc;
using Tms.Api.Dtos;
using Tms.Api.Services;

[ApiController]
[Route("api/students")]

public class StudetnsController(IStudentService studentService) : ControllerBase
{
    // GET/api/students returns all student records
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students = await studentService.GetAllAsync();
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
        var record = await studentService.CreateAsync(request,ct);
        return CreatedAtAction(nameof(GetById), new { id = record?.Id }, record);
    }

    // DELETE /api/students/{id} returns 204 or 404
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await studentService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}