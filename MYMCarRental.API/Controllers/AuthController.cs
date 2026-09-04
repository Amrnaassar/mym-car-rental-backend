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
    private const string AccessTokenCookie = "mym_access_token";
    private const string RefreshTokenCookie = "mym_refresh_token";

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

            SetAuthCookies(result);

            return Ok(new
            {
                user = result.User,
                accessTokenExpiresAt =
                    result.AccessTokenExpiresAt,
                refreshTokenExpiresAt =
                    result.RefreshTokenExpiresAt
            });
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
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken =
            Request.Cookies[RefreshTokenCookie];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized(new
            {
                message = "Refresh token is missing."
            });
        }

        var result =
            await _authService.RefreshTokenAsync(
                refreshToken);

        if (result is null)
        {
            ClearAuthCookies();

            return Unauthorized(new
            {
                message = "Invalid or expired refresh token."
            });
        }

        SetAuthCookies(result);

        return Ok(new
        {
            user = result.User,
            accessTokenExpiresAt =
                result.AccessTokenExpiresAt,
            refreshTokenExpiresAt =
                result.RefreshTokenExpiresAt
        });
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

        ClearAuthCookies();

        return NoContent();
    }


    // ========================================================
    // Cookie Helpers
    // ========================================================

    private void SetAuthCookies(
        AuthResponseDto result)
    {
        var accessTokenOptions =
            CreateCookieOptions(
                result.AccessTokenExpiresAt);

        var refreshTokenOptions =
            CreateCookieOptions(
                result.RefreshTokenExpiresAt);

        Response.Cookies.Append(
            AccessTokenCookie,
            result.AccessToken,
            accessTokenOptions);

        Response.Cookies.Append(
            RefreshTokenCookie,
            result.RefreshToken,
            refreshTokenOptions);
    }


    private CookieOptions CreateCookieOptions(
        DateTime expiresAt)
    {
        return new CookieOptions
        {
            HttpOnly = true,

            Secure = true,

            SameSite = SameSiteMode.None,

            Expires = expiresAt,

            Path = "/"
        };
    }


    private void ClearAuthCookies()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        };

        Response.Cookies.Delete(
            AccessTokenCookie,
            cookieOptions);

        Response.Cookies.Delete(
            RefreshTokenCookie,
            cookieOptions);
    }
}