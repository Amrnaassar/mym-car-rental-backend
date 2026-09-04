using MYMCarRental.Domain.Entities;

namespace MYMCarRental.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);

    DateTime GetAccessTokenExpiration();

    string GenerateRefreshToken();

    string HashRefreshToken(string refreshToken);
}