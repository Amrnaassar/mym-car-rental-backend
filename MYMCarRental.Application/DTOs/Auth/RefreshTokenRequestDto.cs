using System.ComponentModel.DataAnnotations;

namespace MYMCarRental.Application.DTOs.Auth;

public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}