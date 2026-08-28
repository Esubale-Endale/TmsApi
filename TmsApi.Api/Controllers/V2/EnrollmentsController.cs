using MediatR;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TmsApi.Api.Hubs;
using TmsApi.Application.Enrollments.Commands.ApproveEnrollment;
using TmsApi.Application.Enrollments.Commands.EnrollStudent;
using TmsApi.Application.Enrollments.Queries.GetEnrollmentById;
using TmsApi.Application.Enrollments.Queries.GetEnrollments;
using TmsApi.Application.Enrollments.Queries.GetStudentSchedule;
using TmsApi.Application.Hubs;

namespace TmsApi.Api.Controllers.V2;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/enrollments")]
[ApiVersion("2.0")]
public class EnrollmentsController(IMediator mediator, IHubContext<TmsHub, ITmsHubClient> hubContext) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Enroll(EnrollStudentCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.Match<IActionResult>(
            onSuccess: created => CreatedAtAction(nameof(GetSchedule), new { studentId = created.StudentId }, created),
            onFailure: error =>
            {
                var status = error.Code switch
                {
                    "course_not_found" => StatusCodes.Status404NotFound,
                    "course_full" or "already_enrolled" => StatusCodes.Status409Conflict,
                    _ => StatusCodes.Status400BadRequest
                };
                return Problem(
                    statusCode: status,
                    title: "Enrollment rejected",
                    detail: error.Message,
                    type: $"https://tms.local/errors/{error.Code}");
            });
    }

    [HttpGet("{studentId:int}/schedule")]
    public async Task<IActionResult> GetSchedule(int studentId, CancellationToken ct)
    {
        var schedule = await mediator.Send(
        new GetStudentScheduleQuery(studentId), ct);
        return Ok(schedule);
    }

    [HttpPut("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, CancellationToken ct)
    {
        await mediator.Send(new ApproveEnrollmentCommand(id), ct);

        await hubContext.Clients.All.ReceiveEnrollmentStatusUpdated(id.ToString(), "Approved");

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetEnrollmentsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetEnrollmentByIdQuery(id), ct);
        return Ok(result);
    }
}