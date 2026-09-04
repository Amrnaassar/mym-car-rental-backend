using MYMCarRental.Application.DTOs.Locations;

namespace MYMCarRental.Application.Interfaces;

public interface ILocationService
{
    Task<IEnumerable<LocationDto>> GetAllAsync();

    Task<IEnumerable<LocationDto>> GetActiveAsync();

    Task<LocationDto?> GetByIdAsync(int id);

    Task<LocationDto> CreateAsync(CreateLocationDto dto);

    Task<bool> UpdateAsync(
        int id,
        UpdateLocationDto dto);

    Task<bool> DeleteAsync(int id);
}