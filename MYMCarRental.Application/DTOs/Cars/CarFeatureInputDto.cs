using System.ComponentModel.DataAnnotations;

namespace MYMCarRental.Application.DTOs.Cars;

public class CarFeatureInputDto
{
    [Required]
    [MaxLength(500)]
    public string FeatureAr { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FeatureEn { get; set; } = string.Empty;
}   