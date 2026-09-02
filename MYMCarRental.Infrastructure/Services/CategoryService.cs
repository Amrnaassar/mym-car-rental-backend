using Microsoft.EntityFrameworkCore;
using MYMCarRental.Application.DTOs.Categories;
using MYMCarRental.Application.Interfaces;
using MYMCarRental.Domain.Entities;
using MYMCarRental.Infrastructure.Data;

namespace MYMCarRental.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;
    private readonly IImageStorageService _imageStorage;

    public CategoryService(
        AppDbContext context,
        IImageStorageService imageStorage)
    {
        _context = context;
        _imageStorage = imageStorage;
    }

    // =========================
    // Get All
    // =========================

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _context.CarCategories
            .AsNoTracking()
            .OrderBy(c => c.NameEn)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                NameAr = c.NameAr,
                NameEn = c.NameEn,
                Slug = c.Slug,
                DescriptionAr = c.DescriptionAr,
                DescriptionEn = c.DescriptionEn,
                ImageUrl = c.ImageUrl,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync();

        return categories;
    }

    // =========================
    // Get By Id
    // =========================

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        return await _context.CarCategories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                NameAr = c.NameAr,
                NameEn = c.NameEn,
                Slug = c.Slug,
                DescriptionAr = c.DescriptionAr,
                DescriptionEn = c.DescriptionEn,
                ImageUrl = c.ImageUrl,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    // =========================
    // Get By Slug
    // =========================

    public async Task<CategoryDto?> GetBySlugAsync(string slug)
    {
        slug = slug.Trim().ToLower();

        return await _context.CarCategories
            .AsNoTracking()
            .Where(c => c.Slug == slug)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                NameAr = c.NameAr,
                NameEn = c.NameEn,
                Slug = c.Slug,
                DescriptionAr = c.DescriptionAr,
                DescriptionEn = c.DescriptionEn,
                ImageUrl = c.ImageUrl,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    // =========================
    // Create
    // =========================

    public async Task<CategoryDto> CreateAsync(
        CreateCategoryDto dto,
        Stream? imageStream,
        string? imageFileName)
    {
        var nameAr = dto.NameAr.Trim();
        var nameEn = dto.NameEn.Trim();

        var slug = dto.Slug.Trim().ToLower();

        var descriptionAr = dto.DescriptionAr?.Trim();
        var descriptionEn = dto.DescriptionEn?.Trim();

        var slugExists = await _context.CarCategories
            .AnyAsync(c => c.Slug == slug);

        if (slugExists)
        {
            throw new InvalidOperationException(
                "A category with this slug already exists.");
        }

        var category = new CarCategory
        {
            NameAr = nameAr,
            NameEn = nameEn,

            Slug = slug,

            DescriptionAr = descriptionAr,
            DescriptionEn = descriptionEn,

            IsActive = true,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // =========================
        // Upload Category Image
        // =========================

        if (imageStream is not null &&
            !string.IsNullOrWhiteSpace(imageFileName))
        {
            var result = await _imageStorage.UploadAsync(
                imageStream,
                imageFileName,
                "mym-car-rental/categories");

            category.ImageUrl = result.ImageUrl;
            category.ImagePublicId = result.PublicId;
        }

        _context.CarCategories.Add(category);

        await _context.SaveChangesAsync();

        return MapToDto(category);
    }

    // =========================
    // Update
    // =========================

    public async Task<CategoryDto?> UpdateAsync(
        int id,
        UpdateCategoryDto dto,
        Stream? imageStream,
        string? imageFileName)
    {
        var category = await _context.CarCategories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
        {
            return null;
        }

        var nameAr = dto.NameAr.Trim();
        var nameEn = dto.NameEn.Trim();

        var slug = dto.Slug.Trim().ToLower();

        var descriptionAr = dto.DescriptionAr?.Trim();
        var descriptionEn = dto.DescriptionEn?.Trim();

        // =========================
        // Check Slug
        // =========================

        var slugExists = await _context.CarCategories
            .AnyAsync(c =>
                c.Slug == slug &&
                c.Id != id);

        if (slugExists)
        {
            throw new InvalidOperationException(
                "A category with this slug already exists.");
        }

        // Keep old image public id
        var oldPublicId = category.ImagePublicId;

        // =========================
        // Update Data
        // =========================

        category.NameAr = nameAr;
        category.NameEn = nameEn;

        category.Slug = slug;

        category.DescriptionAr = descriptionAr;
        category.DescriptionEn = descriptionEn;

        category.UpdatedAt = DateTime.UtcNow;

        // =========================
        // Replace Image
        // =========================

        if (imageStream is not null &&
            !string.IsNullOrWhiteSpace(imageFileName))
        {
            var result = await _imageStorage.UploadAsync(
                imageStream,
                imageFileName,
                "mym-car-rental/categories");

            category.ImageUrl = result.ImageUrl;
            category.ImagePublicId = result.PublicId;

            await _context.SaveChangesAsync();

            // Delete old image from Cloudinary
            if (!string.IsNullOrWhiteSpace(oldPublicId))
            {
                await _imageStorage.DeleteAsync(oldPublicId);
            }
        }
        else
        {
            await _context.SaveChangesAsync();
        }

        return MapToDto(category);
    }

    // =========================
    // Delete
    // =========================

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.CarCategories
            .Include(c => c.Cars)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
        {
            return false;
        }

        // Cannot delete category if it has cars
        if (category.Cars.Any())
        {
            throw new InvalidOperationException(
                "Cannot delete a category that contains cars.");
        }

        var publicId = category.ImagePublicId;

        _context.CarCategories.Remove(category);

        await _context.SaveChangesAsync();

        // Delete image from Cloudinary
        if (!string.IsNullOrWhiteSpace(publicId))
        {
            await _imageStorage.DeleteAsync(publicId);
        }

        return true;
    }

    // =========================
    // Mapping
    // =========================

    private static CategoryDto MapToDto(CarCategory category)
    {
        return new CategoryDto
        {
            Id = category.Id,

            NameAr = category.NameAr,
            NameEn = category.NameEn,

            Slug = category.Slug,

            DescriptionAr = category.DescriptionAr,
            DescriptionEn = category.DescriptionEn,

            ImageUrl = category.ImageUrl,

            IsActive = category.IsActive,

            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}