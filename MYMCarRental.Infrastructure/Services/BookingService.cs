using System.Data;
using Microsoft.EntityFrameworkCore;
using MYMCarRental.Application.DTOs.Bookings;
using MYMCarRental.Application.Interfaces;
using MYMCarRental.Domain.Entities;
using MYMCarRental.Domain.Enums;
using MYMCarRental.Infrastructure.Data;

namespace MYMCarRental.Infrastructure.Services;

public class BookingService : IBookingService
{
    private const decimal InsurancePrice = 1000m;
    private const decimal TaxRate = 0m;

    private readonly AppDbContext _context;

    public BookingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BookingDto> CreateAsync(
        CreateBookingDto dto,
        Guid? userId)
    {
        if (dto.PickupDate >= dto.ReturnDate)
        {
            throw new InvalidOperationException(
                "Return date must be after pickup date.");
        }

        if (string.IsNullOrWhiteSpace(dto.PickupLocation))
        {
            throw new InvalidOperationException(
                "Pickup location is required.");
        }

        var pickupDate = dto.PickupDate.ToUniversalTime();
        var returnDate = dto.ReturnDate.ToUniversalTime();

        var rentalDays =
            (int)Math.Ceiling(
                (returnDate - pickupDate).TotalDays);

        if (rentalDays <= 0)
        {
            throw new InvalidOperationException(
                "Rental period must be at least one day.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var car = await _context.Cars
            .FirstOrDefaultAsync(c =>
                c.Id == dto.CarId &&
                c.IsActive);

        if (car is null)
        {
            throw new InvalidOperationException(
                "Car not found or inactive.");
        }

        var hasConflict = await _context.Bookings
            .AnyAsync(b =>
                b.CarId == dto.CarId &&
                (
                    b.Status == BookingStatus.Pending ||
                    b.Status == BookingStatus.Confirmed
                ) &&
                b.PickupDate < returnDate &&
                b.ReturnDate > pickupDate);

        if (hasConflict)
        {
            throw new InvalidOperationException(
                "The car is not available for the selected dates.");
        }

        var rentalPlan = CalculateRentalPlan(
            rentalDays);

        var rentalCost = CalculateRentalCost(
            rentalPlan,
            rentalDays,
            car.PricePerDay,
            car.PricePerWeek,
            car.PricePerMonth);

        var insuranceCost =
            dto.IncludeInsurance
                ? InsurancePrice
                : 0m;

        var taxCost =
            CalculateTax(
                rentalCost,
                insuranceCost);

        var discountAmount = 0m;

        var grandTotal =
            rentalCost
            + insuranceCost
            + taxCost
            - discountAmount;

        var booking = new Booking
        {
            Id = Guid.NewGuid(),

            BookingNumber =
                await GenerateBookingNumberAsync(),

            CarId =
                car.Id,

            UserId =
                userId,

            PickupLocation =
                dto.PickupLocation.Trim(),

            PickupDate =
                pickupDate,

            ReturnDate =
                returnDate,

            RentalDays =
                rentalDays,

            RentalPlan =
                rentalPlan,

            CustomerFullName =
                dto.CustomerFullName.Trim(),

            CustomerEmail =
                dto.CustomerEmail
                    .Trim()
                    .ToLowerInvariant(),

            CustomerPhone =
                dto.CustomerPhone.Trim(),

            DrivingLicense =
                dto.DrivingLicense.Trim(),

            DailyRateSnapshot =
                car.PricePerDay,

            WeeklyRateSnapshot =
                car.PricePerWeek,

            MonthlyRateSnapshot =
                car.PricePerMonth,

            RentalCost =
                rentalCost,

            InsuranceCost =
                insuranceCost,

            TaxCost =
                taxCost,

            DiscountAmount =
                discountAmount,

            GrandTotal =
                grandTotal,

            Status =
                BookingStatus.Pending,

            Notes =
                string.IsNullOrWhiteSpace(dto.Notes)
                    ? null
                    : dto.Notes.Trim(),

            CreatedAt =
                DateTime.UtcNow,

            UpdatedAt =
                DateTime.UtcNow
        };

        _context.Bookings.Add(booking);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return await GetByIdAsync(booking.Id)
            ?? throw new InvalidOperationException(
                "Booking could not be loaded.");
    }

    public async Task<BookingDto?> GetByIdAsync(
        Guid id)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BookingDto
            {
                Id =
                    b.Id,

                BookingNumber =
                    b.BookingNumber,

                CarId =
                    b.CarId,

                CarName =
                    b.Car.NameEn,

                PickupLocation =
                    b.PickupLocation,

                PickupDate =
                    b.PickupDate,

                ReturnDate =
                    b.ReturnDate,

                RentalDays =
                    b.RentalDays,

                RentalPlan =
                    b.RentalPlan,

                CustomerFullName =
                    b.CustomerFullName,

                CustomerEmail =
                    b.CustomerEmail,

                CustomerPhone =
                    b.CustomerPhone,

                DailyRate =
                    b.DailyRateSnapshot,

                WeeklyRate =
                    b.WeeklyRateSnapshot,

                MonthlyRate =
                    b.MonthlyRateSnapshot,

                RentalCost =
                    b.RentalCost,

                InsuranceCost =
                    b.InsuranceCost,

                TaxCost =
                    b.TaxCost,

                DiscountAmount =
                    b.DiscountAmount,

                GrandTotal =
                    b.GrandTotal,

                Status =
                    b.Status,

                Notes =
                    b.Notes,

                CreatedAt =
                    b.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<BookingDto>>
        GetMyBookingsAsync(Guid userId)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new BookingDto
            {
                Id =
                    b.Id,

                BookingNumber =
                    b.BookingNumber,

                CarId =
                    b.CarId,

                CarName =
                    b.Car.NameEn,

                PickupLocation =
                    b.PickupLocation,

                PickupDate =
                    b.PickupDate,

                ReturnDate =
                    b.ReturnDate,

                RentalDays =
                    b.RentalDays,

                RentalPlan =
                    b.RentalPlan,

                CustomerFullName =
                    b.CustomerFullName,

                CustomerEmail =
                    b.CustomerEmail,

                CustomerPhone =
                    b.CustomerPhone,

                DailyRate =
                    b.DailyRateSnapshot,

                WeeklyRate =
                    b.WeeklyRateSnapshot,

                MonthlyRate =
                    b.MonthlyRateSnapshot,

                RentalCost =
                    b.RentalCost,

                InsuranceCost =
                    b.InsuranceCost,

                TaxCost =
                    b.TaxCost,

                DiscountAmount =
                    b.DiscountAmount,

                GrandTotal =
                    b.GrandTotal,

                Status =
                    b.Status,

                Notes =
                    b.Notes,

                CreatedAt =
                    b.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<BookingDto>>
        GetAllAsync()
    {
        return await _context.Bookings
            .AsNoTracking()
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new BookingDto
            {
                Id =
                    b.Id,

                BookingNumber =
                    b.BookingNumber,

                CarId =
                    b.CarId,

                CarName =
                    b.Car.NameEn,

                PickupLocation =
                    b.PickupLocation,

                PickupDate =
                    b.PickupDate,

                ReturnDate =
                    b.ReturnDate,

                RentalDays =
                    b.RentalDays,

                RentalPlan =
                    b.RentalPlan,

                CustomerFullName =
                    b.CustomerFullName,

                CustomerEmail =
                    b.CustomerEmail,

                CustomerPhone =
                    b.CustomerPhone,

                DailyRate =
                    b.DailyRateSnapshot,

                WeeklyRate =
                    b.WeeklyRateSnapshot,

                MonthlyRate =
                    b.MonthlyRateSnapshot,

                RentalCost =
                    b.RentalCost,

                InsuranceCost =
                    b.InsuranceCost,

                TaxCost =
                    b.TaxCost,

                DiscountAmount =
                    b.DiscountAmount,

                GrandTotal =
                    b.GrandTotal,

                Status =
                    b.Status,

                Notes =
                    b.Notes,

                CreatedAt =
                    b.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateStatusAsync(
        Guid id,
        BookingStatus status)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking is null)
        {
            return false;
        }

        if (booking.Status == BookingStatus.Cancelled ||
            booking.Status == BookingStatus.Completed)
        {
            throw new InvalidOperationException(
                "The booking status cannot be changed.");
        }

        booking.Status =
            status;

        booking.UpdatedAt =
            DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CancelAsync(
        Guid id,
        Guid? userId,
        bool isStaff)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking is null)
        {
            return false;
        }

        if (!isStaff &&
            booking.UserId != userId)
        {
            return false;
        }

        if (booking.Status == BookingStatus.Completed)
        {
            throw new InvalidOperationException(
                "Completed bookings cannot be cancelled.");
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            return true;
        }

        booking.Status =
            BookingStatus.Cancelled;

        booking.UpdatedAt =
            DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    private static RentalPlan CalculateRentalPlan(
        int rentalDays)
    {
        if (rentalDays < 7)
        {
            return RentalPlan.Daily;
        }

        if (rentalDays < 30)
        {
            return RentalPlan.Weekly;
        }

        return RentalPlan.Monthly;
    }

    private static decimal CalculateRentalCost(
        RentalPlan rentalPlan,
        int rentalDays,
        decimal dailyRate,
        decimal weeklyRate,
        decimal monthlyRate)
    {
        return rentalPlan switch
        {
            RentalPlan.Daily =>
                rentalDays * dailyRate,

            RentalPlan.Weekly =>
                (rentalDays / 7) * weeklyRate
                + (rentalDays % 7) * dailyRate,

            RentalPlan.Monthly =>
                (rentalDays / 30) * monthlyRate
                + (rentalDays % 30) * dailyRate,

            _ => throw new InvalidOperationException(
                "Invalid rental plan.")
        };
    }

    private static decimal CalculateTax(
        decimal rentalCost,
        decimal insuranceCost)
    {
        return
            (rentalCost + insuranceCost)
            * TaxRate;
    }

    private async Task<string>
        GenerateBookingNumberAsync()
    {
        while (true)
        {
            var bookingNumber =
                $"MYM-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}";

            var exists =
                await _context.Bookings
                    .AnyAsync(b =>
                        b.BookingNumber == bookingNumber);

            if (!exists)
            {
                return bookingNumber;
            }
        }
    }
}