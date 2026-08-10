using MediatR;
using TmsApi.Domain.Enums;

namespace TmsApi.Application.Enrollments.Queries.GetStudentSchedule;

public record GetStudentScheduleQuery(int StudentId) : IRequest<ScheduleDto>;
public record ScheduleDto(int StudentId, List<ScheduleItemDto> Courses);
public record ScheduleItemDto(string CourseCode, string Title, string Schedule, EnrollmentStatus Status = EnrollmentStatus.Pending);