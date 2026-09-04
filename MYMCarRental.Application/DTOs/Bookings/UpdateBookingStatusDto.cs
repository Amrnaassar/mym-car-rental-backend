using System.ComponentModel.DataAnnotations;
using MYMCarRental.Domain.Enums;

namespace MYMCarRental.Application.DTOs.Bookings;

public class UpdateBookingStatusDto
{
    [Required]
    public BookingStatus Status { get; set; }
}