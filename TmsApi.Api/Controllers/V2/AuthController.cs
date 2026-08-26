using Asp.Versioning;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TmsApi.Domain.Entities;
using TmsApi.Infrastructure.Identity;
using TmsApi.Infrastructure.Persistence;
using TmsApi.Infrastructure.Services;
using TmsApi.Application.DTOs;

namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("2.0")]
public class AuthController(UserManager<TmsUser> userManager, RoleManager<IdentityRole> roleManager, TmsDbContext context, TokenService tokenService) : ControllerBase
{
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

    [EnableRateLimiting("AuthLimiter")]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null) return Unauthorized(new { detail = "Invalid credentials." });
        if (await userManager.IsLockedOutAsync(user)) return StatusCode(StatusCodes.Status423Locked, new { detail = "Account locked due to multiple failed login attempts. Try again in 15 minutes." });

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            await userManager.AccessFailedAsync(user);
            return Unauthorized(new { detail = "Invalid credentials." });
        }

        await userManager.ResetAccessFailedCountAsync(user);
        var roles = await userManager.GetRolesAsync(user);
        var refreshToken = CreateRefreshToken(user.Id);
        context.RefreshTokens.Add(refreshToken);

        await context.SaveChangesAsync();

        Response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = refreshToken.ExpiresAt
        });

        return Ok(new
        {
            accessToken = tokenService.GenerateJwt(user, roles),
            refreshToken = refreshToken.Token
        });
    }

    public record RefreshRequest(string RefreshToken);

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request)
    {
        var storedToken = await context.RefreshTokens
            .SingleOrDefaultAsync(refreshToken => refreshToken.Token == request.RefreshToken);

        if (storedToken is null) return Unauthorized(new { detail = "Invalid refresh token." });

        if (storedToken.IsUsed)
        {
            var userTokens = await context.RefreshTokens
                .Where(refreshToken => refreshToken.UserId == storedToken.UserId)
                .ToListAsync();

            foreach (var userToken in userTokens)
            {
                userToken.IsRevoked = true;
            }

            await context.SaveChangesAsync();
            return Unauthorized(new { detail = "Token theft detected. All user sessions revoked." });
        }

        if (storedToken.IsRevoked || storedToken.ExpiresAt <= DateTime.UtcNow) return Unauthorized(new { detail = "Refresh token expired or revoked." });

        var user = await userManager.FindByIdAsync(storedToken.UserId);
        if (user is null) return Unauthorized(new { detail = "Invalid refresh token." });

        storedToken.IsUsed = true;
        var newRefreshToken = CreateRefreshToken(user.Id);
        context.RefreshTokens.Add(newRefreshToken);
        await context.SaveChangesAsync();

        Response.Cookies.Append("refreshToken", newRefreshToken.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = newRefreshToken.ExpiresAt
        });

        var roles = await userManager.GetRolesAsync(user);
        return Ok(new
        {
            accessToken = tokenService.GenerateJwt(user, roles),
            refreshToken = newRefreshToken.Token
        });
    }

    [HttpGet("me")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

        if (userId is null) return Unauthorized(new { detail = "Session expired or missing bearer token." });

        var user = await userManager.FindByIdAsync(userId);
        return user is null
            ? Unauthorized(new { detail = "Session expired or missing bearer token." })
            : Ok(new { userId = user.Id, email = user.Email, firstName = user.FirstName, lastName = user.LastName });
    }

    private static RefreshToken CreateRefreshToken(string userId) => new()
    {
        Token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),
        UserId = userId,
        ExpiresAt = DateTime.UtcNow.AddDays(7)
    };
}