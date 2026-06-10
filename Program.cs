
// Starter pipeline (do not assume this order is correct)
using Microsoft.AspNetCore.Authentication;
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions,
    TrainingAuthHandler>("Training", null);

builder.Services.AddAuthorization();
builder.Services.AddSingleton<EnrollmentWorker>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();
// app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/assessments/results", () => Results.Ok(new
{
    courseCode = "CS-101",
    studentId = "S-001",
    letterGrade = "A"
})).RequireAuthorization();

app.MapGet("/api/enrollments/worker-smoke", (EnrollmentWorker worker) =>
{
    worker.ProcessBatch();
    return Results.Ok("processed");
});


// TODO1:Register routing in the pipeline where it belongs for your app.
// TODO2:Register authentication and authorization in the pipeline where your template and facilitator expect them for a protected minimal API route.
// TODO3:MapGET/api/assessments/results with the same response body as the starter, but require authorization for that route.

app.Run();


/*
app.Use(async (context, next) =>
{
    // Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    // Console.Write($"Headers: {string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");

    string authHeader = context.Request.Headers["Authorization"];
    if (!string.IsNullOrEmpty(authHeader))
    {
        Console.WriteLine($"Authorization Header: {authHeader}");
        await next();
    }
    else
    {
        Console.WriteLine("No Authorization header found.");
        context.Response.StatusCode = 401; // Unauthorized
        await context.Response.WriteAsync("Unauthorized: Missing Authorization header.");
    }

});
*/