using MYMCarRental.Domain.Enums;

namespace MYMCarRental.Domain.Entities;

public class Car
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
    // Pricing
    // =========================

    public decimal PricePerDay { get; set; }

    public decimal PricePerWeek { get; set; }

    public decimal PricePerMonth { get; set; }

    // =========================
    // Car Specifications
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

    public bool IsActive { get; set; } = true;

    public bool IsFeatured { get; set; }

    // =========================
    // Audit
    // =========================

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // =========================
    // Relationships
    // =========================

    public CarCategory Category { get; set; } = null!;

    public ICollection<CarImage> Images { get; set; } = new List<CarImage>();

    public ICollection<CarFeatures> Features { get; set; } = new List<CarFeatures>();

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}