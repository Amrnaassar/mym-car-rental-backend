using MYMCarRental.Domain.Enums;

namespace MYMCarRental.Domain.Entities;

public class Booking
{
    public Guid Id { get; set; }

    public string BookingNumber { get; set; } = string.Empty;

    public int CarId { get; set; }

    public Guid? UserId { get; set; }

    public int PickupLocationId { get; set; }

    public DateTime PickupDate { get; set; }

    public DateTime ReturnDate { get; set; }

    public int RentalDays { get; set; }

    public RentalPlan RentalPlan { get; set; }

    public string CustomerFullName { get; set; } = string.Empty;

    public string CustomerEmail { get; set; } = string.Empty;

    public string CustomerPhone { get; set; } = string.Empty;

    public string DrivingLicense { get; set; } = string.Empty;

    public decimal DailyRateSnapshot { get; set; }

    public decimal WeeklyRateSnapshot { get; set; }

    public decimal MonthlyRateSnapshot { get; set; }

    public decimal RentalCost { get; set; }

    public decimal InsuranceCost { get; set; }

    public decimal TaxCost { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal GrandTotal { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Car Car { get; set; } = null!;

    public User? User { get; set; }

    public Location PickupLocation { get; set; } = null!;
}