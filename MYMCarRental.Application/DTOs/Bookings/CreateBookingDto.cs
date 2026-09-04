using System.ComponentModel.DataAnnotations;

namespace MYMCarRental.Application.DTOs.Bookings;

public class CreateBookingDto
{
    [Required]
    public int CarId { get; set; }

    [Required]
    public int PickupLocationId { get; set; }

    [Required]
    public DateTime PickupDate { get; set; }

    [Required]
    public DateTime ReturnDate { get; set; }

    [Required]
    [MaxLength(150)]
    public string CustomerFullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string CustomerPhone { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string DrivingLicense { get; set; } = string.Empty;

    public bool IncludeInsurance { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}