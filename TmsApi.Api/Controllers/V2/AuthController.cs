using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TmsApi.Infrastructure.Identity;

namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/{version:apiVersion}/auth")]
[ApiVersion("2.0")]
public class AuthController(
    UserManager<TmsUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IWebHostEnvironment environment) : ControllerBase
{
    public record RegisterRequest(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string Role);

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        
        if (existingUser is not null) return Ok(new { message = "Registration request received." });

        var user = new TmsUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded) return BadRequest(new { errors = result.Errors.Select(error => error.Description) });
        if (!await roleManager.RoleExistsAsync(request.Role)) await roleManager.CreateAsync(new IdentityRole(request.Role));

        await userManager.AddToRoleAsync(user, request.Role);
        return Ok(new { message = "Registration successful." });
    }

    public record LoginRequest(string Email, string Password);

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Unauthorized(new { detail = "Invalid credentials." });
        }

        if (await userManager.IsLockedOutAsync(user))
        {
            return StatusCode(StatusCodes.Status423Locked, new
            {
                detail = "Account locked due to multiple failed login attempts. Try again in 15 minutes."
            });
        }

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            await userManager.AccessFailedAsync(user);
            return Unauthorized(new { detail = "Invalid credentials." });
        }

        await userManager.ResetAccessFailedCountAsync(user);
        Response.Cookies.Append("tms_auth", user.Id, new CookieOptions
        {
            HttpOnly = true,
            Secure = !environment.IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(2)
        });

        return Ok(new
        {
            userId = user.Id,
            email = user.Email,
            firstName = user.FirstName,
            lastName = user.LastName
        });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        if (!Request.Cookies.TryGetValue("tms_auth", out var userId))
        {
            return Unauthorized(new { detail = "Session expired or missing authentication cookie." });
        }

        var user = await userManager.FindByIdAsync(userId);
        return user is null
            ? Unauthorized(new { detail = "Session expired or missing authentication cookie." })
            : Ok(new { userId = user.Id, email = user.Email, firstName = user.FirstName, lastName = user.LastName });
    }
}