// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Routing;
// using TmsApi.Application.DTOs;
// using TmsApi.Application.Interfaces;

// [ApiController]
// [Route("api/courses")]
// [Tags("Courses")]
// [Produces("Application/json")]
// [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
// public class CoursesController(ICourseService courseService, LinkGenerator linkGenerator) : ControllerBase
// {
//     [HttpGet]
//     [ProducesResponseType(typeof(PagedResponse<CourseResponseDto>), StatusCodes.Status200OK)]
//     [EndpointSummary("List courses with pagination")]
//     [EndpointDescription("Returns a paginated, optionally filtered list of TMS courses. PageSize is capped at 50.")]
//     public async Task<IActionResult> GetAll([FromQuery] PagedRequest request, CancellationToken ct)
//     {
//         var result = await courseService.GetCoursesAsync(request, ct);
//         return Ok(result);
//     }

//     [HttpGet("{id:int}", Name = nameof(GetCourseById))]
//     [ProducesResponseType(typeof(CourseDetailDto), StatusCodes.Status200OK)]
//     [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
//     [EndpointSummary("Get a course by ID")]
//     [EndpointDescription("Returns course details with HATEOAS links. Returns 404 if the course does not exist.")]
//     public async Task<IActionResult> GetCourseById(int id, CancellationToken ct) 
//     {
//         var course = await courseService.GetByIdAsync(id, ct);

//         if (course is null) return NotFound();

//         var self = linkGenerator.GetPathByName( 
//             HttpContext,
//             nameof(GetCourseById),
//             new { id = course.Id })!;
//         var enrollments = linkGenerator.GetPathByAction(
//             HttpContext,
//             action: "GetEnrollments",
//             controller: "Enrollments",
//             values: new { courseId = course.Id })!;
        
//         var links = new List<LinkDto>
//         {
//             new(self, "self", "GET"),
//             new(self, "update", "PUT"),
//             new(self, "delete", "DELETE"),
//             new(enrollments, "enrollments", "GET")
//         };

//         if (course.EnrollmentCount < course.MaxCapacity)
//         {
//             links.Add(
//                 new LinkDto(
//                     enrollments,
//                     "enroll",
//                     "POST"));
//         }

//         var detail = new CourseDetailDto
//         {
//             Id = course.Id,
//             Code = course.Code,
//             Title = course.Title,
//             MaxCapacity = course.MaxCapacity,
//             EnrollmentCount = course.EnrollmentCount,
//             Links = links
//         };

//     return Ok(detail);

//     }
  
//     [HttpPost]
//     [ProducesResponseType(typeof(CourseResponseDto), StatusCodes.Status201Created)]
//     [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
//     [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
//     [EndpointSummary("Create a new course")]
//     [EndpointDescription("Creates a course with a unique code. Returns409 if the course code already exists.")]
//     public async Task<IActionResult> Create(CreateCourseRequest request, CancellationToken ct)
//     {
//         if (await courseService.CodeExistsAsync(request.Code, ct))
//         {
//             return Conflict(new ProblemDetails
//             {
//                 Title = "Course code already exists",
//                 Detail = $"A course with the code '{request.Code}' already Registered.",
//                 Status = StatusCodes.Status409Conflict,
//             });
//         }
//         var result = await courseService.CreateAsync(request, ct);

//         return CreatedAtAction(
//             nameof(GetCourseById),
//             new { id = result.Id },
//             result);
//     }
//     [HttpDelete("{id:int}")]
//     public async Task<IActionResult> Delete(int id)
//     {
//         var deleted = await courseService.DeleteAsync(id);
//         return deleted ? NoContent() : NotFound();
//     }
// }