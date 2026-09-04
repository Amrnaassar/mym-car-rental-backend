using MYMCarRental.Domain.Enums;

namespace MYMCarRental.Application.DTOs.Bookings;

public class BookingDto
{
    public Guid Id { get; set; }

    public string BookingNumber { get; set; } = string.Empty;

    public int CarId { get; set; }

    public string CarName { get; set; } = string.Empty;

    public int PickupLocationId { get; set; }

    public string PickupLocationName { get; set; } = string.Empty;

    public DateTime PickupDate { get; set; }

    public DateTime ReturnDate { get; set; }

    public int RentalDays { get; set; }

    public RentalPlan RentalPlan { get; set; }

    public string CustomerFullName { get; set; } = string.Empty;

    public string CustomerEmail { get; set; } = string.Empty;

    public string CustomerPhone { get; set; } = string.Empty;

    public decimal DailyRate { get; set; }

    public decimal WeeklyRate { get; set; }

    public decimal MonthlyRate { get; set; }

    public decimal RentalCost { get; set; }

    public decimal InsuranceCost { get; set; }

    public decimal TaxCost { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal GrandTotal { get; set; }

    public BookingStatus Status { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
}