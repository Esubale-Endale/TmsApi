// using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc;
using TmsApi.Data;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/dashboard")]

public class DashboardController(TmsDbContext context) : ControllerBase
{

    //  Howmanyactive students have GPA >= 3.0?
    // public IActionResult GoodStandingStudent()
    // {
    //     Console.WriteLine("+----------------------------------------------+");
    //     Console.WriteLine("+----------------------------------------------+");
    // }
}



