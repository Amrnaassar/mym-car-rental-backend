using MYMCarRental.Domain.Enums;

namespace MYMCarRental.Domain.Entities;

public class Car
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal PricePerDay { get; set; }

    public decimal PricePerWeek { get; set; }

    public decimal PricePerMonth { get; set; }

    public Transmission Transmission { get; set; }

    public FuelType FuelType { get; set; }

    public int Seats { get; set; }

    public int Doors { get; set; }

    public int Luggage { get; set; }

    public decimal Rating { get; set; }

    public int ReviewsCount { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsFeatured { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public CarCategory Category { get; set; } = null!;

    public ICollection<CarImage> Images { get; set; } = new List<CarImage>();

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}