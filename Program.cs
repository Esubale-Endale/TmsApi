using Microsoft.AspNetCore.Authentication.Cookies; // Added for scheme definition

var builder = WebApplication.CreateBuilder(args);

// FIX 1: Provide a default authentication scheme so the authorization layer knows how to issue a 401 challenge
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(); // Registers the handler for the default scheme

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

// Register custom logging middleware FIRST (outer wrapper)
app.UseMiddleware<AddRequestLoggingMiddleware>();

// Standard exception handling and HTTPS redirection
app.UseExceptionHandler("/error");
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// FIX 2: Define the missing fallback endpoint for your exception handler path
app.Map("/error", (HttpContext context) =>
{
    return Results.Problem("An unexpected error occurred while processing your request.");
});

// Map endpoints LAST
app.MapGet("/api/assessments/results", () => Results.Ok(new
{
    courseCode = "CS-101",
    studentId = "S-001",
    letterGrade = "A"
}))
.RequireAuthorization();

app.Run();