using System.ComponentModel.DataAnnotations;
using MYMCarRental.Domain.Enums;

namespace MYMCarRental.Application.DTOs.Cars;

public class UpdateCarDto
{
    [Required]
    public int CategoryId { get; set; }

    // =========================
    // Multilingual Content
    // =========================

    [Required]
    [MaxLength(150)]
    public string NameAr { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string NameEn { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? DescriptionAr { get; set; }

    [MaxLength(1000)]
    public string? DescriptionEn { get; set; }

    // =========================
    // Pricing
    // =========================

    [Range(0, double.MaxValue)]
    public decimal PricePerDay { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PricePerWeek { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PricePerMonth { get; set; }

    // =========================
    // Specifications
    // =========================

    public Transmission Transmission { get; set; }

    public FuelType FuelType { get; set; }

    [Range(1, 20)]
    public int Seats { get; set; }

    [Range(1, 10)]
    public int Doors { get; set; }

    [Range(0, 20)]
    public int Luggage { get; set; }

    // =========================
    // Features
    // =========================

    public List<CarFeatureInputDto> Features { get; set; } = new();

    // =========================
    // Status
    // =========================

    public bool IsActive { get; set; }

    public bool IsFeatured { get; set; }
}