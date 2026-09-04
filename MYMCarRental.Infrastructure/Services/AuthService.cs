using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MYMCarRental.Application.DTOs.Auth;
using MYMCarRental.Application.DTOs.Users;
using MYMCarRental.Application.Interfaces;
using MYMCarRental.Application.Settings;
using MYMCarRental.Domain.Entities;
using MYMCarRental.Infrastructure.Data;

namespace MYMCarRental.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;
    private readonly IConfiguration _configuration;

    public AuthService(
        AppDbContext context,
        IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings,
        IConfiguration configuration)
    {
        _context = context;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> GoogleLoginAsync(
        GoogleLoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.IdToken))
        {
            throw new UnauthorizedAccessException(
                "Google ID token is required.");
        }

        var clientId = _configuration["Google:ClientId"];

        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new InvalidOperationException(
                "Google ClientId is not configured.");
        }

        GoogleJsonWebSignature.Payload payload;

        try
        {
            payload = await GoogleJsonWebSignature
                .ValidateAsync(
                    dto.IdToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { clientId }
                    });
        }
        catch
        {
            throw new UnauthorizedAccessException(
                "Invalid Google ID token.");
        }

        if (string.IsNullOrWhiteSpace(payload.Subject) ||
            string.IsNullOrWhiteSpace(payload.Email))
        {
            throw new UnauthorizedAccessException(
                "Invalid Google account information.");
        }

        if (!payload.EmailVerified)
        {
            throw new UnauthorizedAccessException(
                "Google email is not verified.");
        }

        var googleId = payload.Subject;

        var email = payload.Email
            .Trim()
            .ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.GoogleId == googleId);

        if (user is null)
        {
            user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),

                GoogleId = googleId,

                FullName = string.IsNullOrWhiteSpace(payload.Name)
                    ? email.Split('@')[0]
                    : payload.Name.Trim(),

                Email = email,

                Role = Domain.Enums.UserRole.Customer,

                CreatedAt = DateTime.UtcNow,

                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
        }
        else
        {
            user.GoogleId = googleId;

            if (!string.IsNullOrWhiteSpace(payload.Name))
            {
                user.FullName = payload.Name.Trim();
            }

            user.Email = email;
            user.UpdatedAt = DateTime.UtcNow;
        }

        var refreshToken =
            _jwtService.GenerateRefreshToken();

        user.RefreshTokenHash =
            _jwtService.HashRefreshToken(refreshToken);

        user.RefreshTokenExpiresAt =
            DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenDays);

        await _context.SaveChangesAsync();

        var accessToken =
            _jwtService.GenerateAccessToken(user);

        var accessTokenExpiresAt =
            _jwtService.GetAccessTokenExpiration();

        return new AuthResponseDto
        {
            AccessToken = accessToken,

            RefreshToken = refreshToken,

            AccessTokenExpiresAt =
                accessTokenExpiresAt,

            RefreshTokenExpiresAt =
                user.RefreshTokenExpiresAt.Value,

            User = MapToDto(user)
        };
    }

    public async Task<AuthResponseDto?> RefreshTokenAsync(
        string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return null;
        }

        var refreshTokenHash =
            _jwtService.HashRefreshToken(refreshToken);

        var user = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.RefreshTokenHash == refreshTokenHash);

        if (user is null)
        {
            return null;
        }

        if (user.RefreshTokenExpiresAt is null ||
            user.RefreshTokenExpiresAt <= DateTime.UtcNow)
        {
            user.RefreshTokenHash = null;
            user.RefreshTokenExpiresAt = null;

            await _context.SaveChangesAsync();

            return null;
        }

        var newRefreshToken =
            _jwtService.GenerateRefreshToken();

        user.RefreshTokenHash =
            _jwtService.HashRefreshToken(
                newRefreshToken);

        user.RefreshTokenExpiresAt =
            DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenDays);

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var accessToken =
            _jwtService.GenerateAccessToken(user);

        var accessTokenExpiresAt =
            _jwtService.GetAccessTokenExpiration();

        return new AuthResponseDto
        {
            AccessToken = accessToken,

            RefreshToken = newRefreshToken,

            AccessTokenExpiresAt =
                accessTokenExpiresAt,

            RefreshTokenExpiresAt =
                user.RefreshTokenExpiresAt.Value,

            User = MapToDto(user)
        };
    }

    public async Task<bool> LogoutAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return false;
        }

        user.RefreshTokenHash = null;

        user.RefreshTokenExpiresAt = null;

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<UserDto?> GetCurrentUserAsync(
        Guid userId)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}