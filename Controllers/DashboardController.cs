using Microsoft.AspNetCore.Mvc;
using TmsApi.Data;

namespace TmsApi.Controllers;
[ApiController]
[Route("api/dashboard")]
public class DashboardController(TmsDbContext context) : ControllerBase
{
    //  Howmanyactive students have GPA >= 3.0?
    [HttpGet("good-Standing-Students")]
    public IActionResult GoodStandingStudent()
    {
        Console.WriteLine("+-----------Good Standing Students-------------+");
        var query = context.Students.Where(s => s.GPA >= 3.0m && s.IsActive);
        var results = query.ToList();
        Console.WriteLine(query);
        Console.WriteLine("+----------------------------------------------+");
        return Ok(results);
    }
    // Which courses have the most enrollments, sorted descending?
    [HttpGet("most-enrolled-courses")]
    public IActionResult  MostEnrolledCourses()
    {        
        var list =  context.Courses
        .Select(c => new
        {
            c.Title,
            EnrollmentCount = c.Enrollments.Count
        })
        .OrderByDescending(x => x.EnrollmentCount);

        var result = list.ToList();
        return Ok(list);
    }

    // 3. What is the average GPA per course?
    [HttpGet("avgpapercourse")]
    public IActionResult AvgGpaPercourse()
    {
                
        var list =  context.Enrollments
        .GroupBy(e => e.Course.Title)
        .Select(g => new
        {
        Course = g.Key,
        AverageGPA = g.Average(e => e.Student.GPA)
        })
        .ToList();

        return Ok(list);
    }
    // 4. Which students have zero enrollments? (Show both patterns)
    // o Approach A(Using Subquery):
    [HttpGet("zeroEnrollmentsubquery")]
    public IActionResult ZeroEnrolledStudentsSubQuery()
    {
                
        var list =  context.Students
        .Where(s => !s.Enrollments.Any())
        .Select(s => s.Name)
        .ToList();
        return Ok(list);
           }
    // o Approach B(Using EF Core 10 LeftJoin):
    [HttpGet("zeroEnrollmentleftjoin")]
    public IActionResult ZeroEnrolledStudentsLeftJoin()
    {
    var list =  context.Students
    .LeftJoin(context.Enrollments,
    s => s.Id,
    e => e.StudentId,
    (s, e) => new { s, e })
    .Where(x => x.e == null)
    .Select(x => x.s.Name)
    .ToList();
    return Ok(list);
    }
}