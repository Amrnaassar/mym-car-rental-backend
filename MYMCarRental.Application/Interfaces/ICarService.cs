using MYMCarRental.Application.DTOs.Cars;

namespace MYMCarRental.Application.Interfaces;

public interface ICarService
{
    Task<IEnumerable<CarDto>> GetAllAsync();

    Task<IEnumerable<CarDto>> GetFeaturedAsync();

    Task<CarDto?> GetByIdAsync(int id);

    Task<CarDto> CreateAsync(
        CreateCarDto dto,
        IEnumerable<(Stream Stream, string FileName)> images);

    Task<CarDto?> UpdateAsync(
        int id,
        UpdateCarDto dto,
        IEnumerable<(Stream Stream, string FileName)> newImages);

    Task<bool> DeleteAsync(int id);

    Task<bool> DeleteImageAsync(int carId, int imageId);

    Task<bool> SetPrimaryImageAsync(int carId, int imageId);
}