using MYMCarRental.Application.DTOs.Categories;

namespace MYMCarRental.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();

    Task<CategoryDto?> GetByIdAsync(int id);

    Task<CategoryDto?> GetBySlugAsync(string slug);

    Task<CategoryDto> CreateAsync(
        CreateCategoryDto dto,
        Stream? imageStream,
        string? imageFileName);

    Task<CategoryDto?> UpdateAsync(
        int id,
        UpdateCategoryDto dto,
        Stream? imageStream,
        string? imageFileName);

    Task<bool> DeleteAsync(int id);
}