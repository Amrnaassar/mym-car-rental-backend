using MYMCarRental.Application.DTOs.Auth;
using MYMCarRental.Application.DTOs.Users;

namespace MYMCarRental.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> GoogleLoginAsync(
        GoogleLoginDto dto);

    Task<AuthResponseDto?> RefreshTokenAsync(
        string refreshToken);

    Task<bool> LogoutAsync(
        Guid userId);

    Task<UserDto?> GetCurrentUserAsync(
        Guid userId);
}