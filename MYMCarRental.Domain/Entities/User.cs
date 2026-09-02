using MYMCarRental.Domain.Enums;

namespace MYMCarRental.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string GoogleId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public UserRole Role { get; set; } = UserRole.Customer;

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}