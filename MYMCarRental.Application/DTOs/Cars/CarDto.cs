using MYMCarRental.Domain.Enums;

namespace MYMCarRental.Application.DTOs.Cars;

public class CarDto
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    // =========================
    // Multilingual Content
    // =========================

    public string NameAr { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public string? DescriptionAr { get; set; }

    public string? DescriptionEn { get; set; }

    // =========================
    // Category
    // =========================

    public string CategoryNameAr { get; set; } = string.Empty;

    public string CategoryNameEn { get; set; } = string.Empty;

    // =========================
    // Pricing
    // =========================

    public decimal PricePerDay { get; set; }

    public decimal PricePerWeek { get; set; }

    public decimal PricePerMonth { get; set; }

    // =========================
    // Specifications
    // =========================

    public Transmission Transmission { get; set; }

    public FuelType FuelType { get; set; }

    public int Seats { get; set; }

    public int Doors { get; set; }

    public int Luggage { get; set; }

    // =========================
    // Reviews
    // =========================

    public decimal Rating { get; set; }

    public int ReviewsCount { get; set; }

    // =========================
    // Status
    // =========================

    public bool IsActive { get; set; }

    public bool IsFeatured { get; set; }

    // =========================
    // Features
    // =========================

    public List<CarFeatureDto> Features { get; set; } = new();

    // =========================
    // Images
    // =========================

    public string? PrimaryImageUrl { get; set; }

    public List<CarImageDto> Images { get; set; } = new();

    // =========================
    // Audit
    // =========================

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}