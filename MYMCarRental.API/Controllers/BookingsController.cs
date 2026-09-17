using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MYMCarRental.Application.DTOs.Bookings;
using MYMCarRental.Application.Interfaces;
using MYMCarRental.Domain.Enums;
using System.Security.Claims;

namespace MYMCarRental.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(
        IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // =========================
    // POST: api/bookings
    // Guest + Authenticated User
    // =========================

    [HttpPost]
    [Authorize]
    [EnableRateLimiting("GeneralLimiter")]

    public async Task<IActionResult> Create(
        [FromBody] CreateBookingDto dto)
    {
        try
        {
            Guid? userId = null;

            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(
                userIdClaim,
                out var parsedUserId))  
            {
                userId = parsedUserId;
            }

            var booking =
                await _bookingService.CreateAsync(
                    dto,
                    userId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = booking.Id },
                booking);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    // =========================
    // GET: api/bookings/my
    // Customer
    // =========================

    [HttpGet("my")]
    [Authorize]
    [EnableRateLimiting("GeneralLimiter")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userIdClaim =
            User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse( userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var bookings =
            await _bookingService
                .GetMyBookingsAsync(userId);

        return Ok(bookings);
    }

    // =========================
    // GET: api/bookings
    // Employee + Manager
    // =========================

    [HttpGet]
    [Authorize(Roles = "Employee,Manager")]
    [EnableRateLimiting("GeneralLimiter")]
    public async Task<IActionResult> GetAll()
    {
        var bookings =
            await _bookingService.GetAllAsync();

        return Ok(bookings);
    }

    // =========================
    // GET: api/bookings/{id}
    // Employee + Manager
    // =========================

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Employee,Manager")]
    [EnableRateLimiting("GeneralLimiter")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var booking =
            await _bookingService.GetByIdAsync(id);

        if (booking is null)
        {
            return NotFound(new
            {
                message = "Booking not found."
            });
        }

        return Ok(booking);
    }

    // =========================
    // PUT: api/bookings/{id}/status
    // Employee + Manager
    // =========================

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Employee,Manager")]
    [EnableRateLimiting("GeneralLimiter")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateBookingStatusDto dto)
    {
        try
        {
            var updated =
                await _bookingService
                    .UpdateStatusAsync(
                        id,
                        dto.Status);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Booking not found."
                });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    // =========================
    // DELETE:
    // api/bookings/{id}/cancel
    // Customer / Employee / Manager
    // =========================

    [HttpDelete("{id:guid}/cancel")]
    [Authorize]
    [EnableRateLimiting("GeneralLimiter")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var userIdClaim =
            User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

        Guid? userId = null;

        if (Guid.TryParse(
            userIdClaim,
            out var parsedUserId))
        {
            userId = parsedUserId;
        }

        var isStaff =
            User.IsInRole("Employee") ||
            User.IsInRole("Manager");

        try
        {
            var cancelled =
                await _bookingService.CancelAsync(
                    id,
                    userId,
                    isStaff);

            if (!cancelled)
            {
                return NotFound(new
                {
                    message =
                        "Booking not found."
                });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }
}