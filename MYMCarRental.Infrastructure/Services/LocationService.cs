using Microsoft.EntityFrameworkCore;
using MYMCarRental.Application.DTOs.Locations;
using MYMCarRental.Application.Interfaces;
using MYMCarRental.Domain.Entities;
using MYMCarRental.Infrastructure.Data;

namespace MYMCarRental.Infrastructure.Services;

public class LocationService : ILocationService
{
    private readonly AppDbContext _context;

    public LocationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LocationDto>> GetAllAsync()
    {
        return await _context.Locations
            .AsNoTracking()
            .OrderBy(l => l.Name)
            .Select(l => new LocationDto
            {
                Id = l.Id,
                Name = l.Name,
                Address = l.Address,
                City = l.City,
                IsActive = l.IsActive
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<LocationDto>> GetActiveAsync()
    {
        return await _context.Locations
            .AsNoTracking()
            .Where(l => l.IsActive)
            .OrderBy(l => l.Name)
            .Select(l => new LocationDto
            {
                Id = l.Id,
                Name = l.Name,
                Address = l.Address,
                City = l.City,
                IsActive = l.IsActive
            })
            .ToListAsync();
    }

    public async Task<LocationDto?> GetByIdAsync(int id)
    {
        return await _context.Locations
            .AsNoTracking()
            .Where(l => l.Id == id)
            .Select(l => new LocationDto
            {
                Id = l.Id,
                Name = l.Name,
                Address = l.Address,
                City = l.City,
                IsActive = l.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<LocationDto> CreateAsync(
        CreateLocationDto dto)
    {
        var name = dto.Name.Trim();

        var locationExists = await _context.Locations
            .AnyAsync(l => l.Name == name);

        if (locationExists)
        {
            throw new InvalidOperationException(
                "A location with the same name already exists.");
        }

        var location = new Location
        {
            Name = name,

            Address = string.IsNullOrWhiteSpace(dto.Address)
                ? null
                : dto.Address.Trim(),

            City = string.IsNullOrWhiteSpace(dto.City)
                ? null
                : dto.City.Trim(),

            IsActive = true
        };

        _context.Locations.Add(location);

        await _context.SaveChangesAsync();

        return new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Address = location.Address,
            City = location.City,
            IsActive = location.IsActive
        };
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateLocationDto dto)
    {
        var location = await _context.Locations
            .FirstOrDefaultAsync(l => l.Id == id);

        if (location is null)
        {
            return false;
        }

        var name = dto.Name.Trim();

        var duplicateName = await _context.Locations
            .AnyAsync(l =>
                l.Id != id &&
                l.Name == name);

        if (duplicateName)
        {
            throw new InvalidOperationException(
                "A location with the same name already exists.");
        }

        location.Name = name;

        location.Address =
            string.IsNullOrWhiteSpace(dto.Address)
                ? null
                : dto.Address.Trim();

        location.City =
            string.IsNullOrWhiteSpace(dto.City)
                ? null
                : dto.City.Trim();

        location.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var location = await _context.Locations
            .FirstOrDefaultAsync(l => l.Id == id);

        if (location is null)
        {
            return false;
        }

        var hasBookings = await _context.Bookings
            .AnyAsync(b => b.PickupLocationId == id);

        if (hasBookings)
        {
            throw new InvalidOperationException(
                "This location cannot be deleted because it has bookings.");
        }

        _context.Locations.Remove(location);

        await _context.SaveChangesAsync();

        return true;
    }
}