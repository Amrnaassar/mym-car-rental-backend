using MYMCarRental.Application.DTOs.Bookings;
using MYMCarRental.Domain.Enums;

namespace MYMCarRental.Application.Interfaces;

public interface IBookingService
{
    Task<BookingDto> CreateAsync(
        CreateBookingDto dto,
        Guid? userId);

    Task<BookingDto?> GetByIdAsync(
        Guid id);

    Task<IEnumerable<BookingDto>> GetMyBookingsAsync(
        Guid userId);

    Task<IEnumerable<BookingDto>> GetAllAsync();

    Task<bool> UpdateStatusAsync(
        Guid id,
        BookingStatus status);

    Task<bool> CancelAsync(
        Guid id,
        Guid? userId,
        bool isStaff);
}