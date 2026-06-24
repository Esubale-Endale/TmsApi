using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController(TmsDbContext context) : ControllerBase
{
    // 1. Paged Students
    [HttpGet("students")]
    public async Task<IActionResult> GetPagedStudents( int page = 1, CancellationToken cancellationToken = default)
    {
        const int pageSize = 20;

        var students = await context.Students
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Ok(students);
    }

    // 2. Top 5 Courses by Enrollment Count
    [HttpGet("top-5-courses")]
    public async Task<IActionResult> TopCourses( CancellationToken cancellationToken = default)
    {
        var result = await context.Enrollments
            .GroupBy(e => e.Course.Title)
            .Select(g => new
            {
                CourseTitle = g.Key,
                EnrollmentCount = g.Count()
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .Take(5)
            .ToListAsync(cancellationToken);

        return Ok(result);
    }

    // Good Standing Students
    [HttpGet("good-standing-students")]
    public async Task<IActionResult> GoodStandingStudent( CancellationToken cancellationToken = default)
    {
        var results = await context.Students
            .Where(s => s.GPA >= 3.0m && s.IsActive)
            .ToListAsync(cancellationToken);

        return Ok(results);
    }

    // Most Enrolled Courses
    [HttpGet("most-enrolled-courses")]
    public async Task<IActionResult> MostEnrolledCourses( CancellationToken cancellationToken = default)
    {
        var result = await context.Courses
            .Select(c => new
            {
                c.Title,
                EnrollmentCount = c.Enrollments.Count
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .ToListAsync(cancellationToken);

        return Ok(result);
    }

    // Average GPA Per Course
    [HttpGet("avg-gpa-per-course")]
    public async Task<IActionResult> AvgGpaPerCourse( CancellationToken cancellationToken = default)
    {
        var result = await context.Enrollments
            .GroupBy(e => e.Course.Title)
            .Select(g => new
            {
                Course = g.Key,
                AverageGPA = g.Average(e => e.Student.GPA)
            })
            .ToListAsync(cancellationToken);

        return Ok(result);
    }

    // Students With No Enrollments - Subquery
    [HttpGet("zero-enrollment-subquery")]
    public async Task<IActionResult> ZeroEnrolledStudentsSubQuery(
        CancellationToken cancellationToken = default)
    {
        var result = await context.Students
            .Where(s => !s.Enrollments.Any())
            .Select(s => s.Name)
            .ToListAsync(cancellationToken);

        return Ok(result);
    }

    // Students With No Enrollments - Left Join
    [HttpGet("zero-enrollment-leftjoin")]
    public async Task<IActionResult> ZeroEnrolledStudentsLeftJoin(
        CancellationToken cancellationToken = default)
    {
        var result = await context.Students
            .GroupJoin(
                context.Enrollments,
                s => s.Id,
                e => e.StudentId,
                (s, enrollments) => new
                {
                    Student = s,
                    Enrollments = enrollments
                })
            .Where(x => !x.Enrollments.Any())
            .Select(x => x.Student.Name)
            .ToListAsync(cancellationToken);

        return Ok(result);
    }
}