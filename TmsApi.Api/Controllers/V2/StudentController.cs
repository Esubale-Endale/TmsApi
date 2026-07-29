using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TmsApi.Application.Students.Queries.GetStudents;

namespace TmsApi.Api.Controllers.V2;

[ApiController]
[ApiExplorerSettings(GroupName = "v2")]
[Route("api/V{version:apiVersion}/students")]
[ApiVersion("2.0")]
public class StudentController(IMediator mediator) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetStudents(
     CancellationToken cancellationToken)
    {
        var students = await mediator.Send(
            new GetStudentsQuery(),
            cancellationToken);

        return Ok(students);
    }
}