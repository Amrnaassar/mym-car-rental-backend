using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MYMCarRental.Application.DTOs.Auth;
using MYMCarRental.Application.Interfaces;

namespace MYMCarRental.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }


    // ========================================================
    // Google Login
    // ========================================================

    [HttpPost("google")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleLogin(
        [FromBody] GoogleLoginDto dto)
    {
        try
        {
            var result =
                await _authService.GoogleLoginAsync(dto);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
    }


    // ========================================================
    // Refresh Token
    // ========================================================

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequestDto dto)
    {
        var result =
            await _authService.RefreshTokenAsync(
                dto.RefreshToken);

        if (result is null)
        {
            return Unauthorized(new
            {
                message = "Invalid or expired refresh token."
            });
        }

        return Ok(result);
    }


    // ========================================================
    // Current User
    // ========================================================

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userIdClaim =
            User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(
                userIdClaim,
                out var userId))
        {
            return Unauthorized();
        }

        var user =
            await _authService.GetCurrentUserAsync(
                userId);

        if (user is null)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        return Ok(user);
    }


    // ========================================================
    // Logout
    // ========================================================

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userIdClaim =
            User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(
                userIdClaim,
                out var userId))
        {
            return Unauthorized();
        }

        var loggedOut =
            await _authService.LogoutAsync(userId);

        if (!loggedOut)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        return NoContent();
    }
}