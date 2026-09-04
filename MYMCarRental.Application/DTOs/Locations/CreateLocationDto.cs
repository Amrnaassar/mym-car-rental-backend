using System.ComponentModel.DataAnnotations;

namespace MYMCarRental.Application.DTOs.Locations;

public class CreateLocationDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }
}