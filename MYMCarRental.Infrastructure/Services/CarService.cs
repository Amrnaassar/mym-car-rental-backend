using Microsoft.EntityFrameworkCore;
using MYMCarRental.Application.DTOs.Cars;
using MYMCarRental.Application.Interfaces;
using MYMCarRental.Domain.Entities;
using MYMCarRental.Infrastructure.Data;

namespace MYMCarRental.Infrastructure.Services;

public class CarService : ICarService
{
    private readonly AppDbContext _context;
    private readonly IImageStorageService _imageStorage;

    public CarService(
        AppDbContext context,
        IImageStorageService imageStorage)
    {
        _context = context;
        _imageStorage = imageStorage;
    }

    // =========================
    // Get All
    // =========================

    public async Task<IEnumerable<CarDto>> GetAllAsync()
    {
        var cars = await _context.Cars
            .AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Images)
            .OrderBy(c => c.NameEn)
            .ToListAsync();

        return cars.Select(MapToDto);
    }

    // =========================
    // Get Featured
    // =========================

    public async Task<IEnumerable<CarDto>> GetFeaturedAsync()
    {
        var cars = await _context.Cars
            .AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Images)
            .Where(c => c.IsActive && c.IsFeatured)
            .OrderBy(c => c.NameEn)
            .ToListAsync();

        return cars.Select(MapToDto);
    }

    // =========================
    // Get By Id
    // =========================

    public async Task<CarDto?> GetByIdAsync(int id)
    {
        var car = await _context.Cars
            .AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.Id == id);

        return car is null ? null : MapToDto(car);
    }

    // =========================
    // Create
    // =========================

    public async Task<CarDto> CreateAsync(
        CreateCarDto dto,
        IEnumerable<(Stream Stream, string FileName)> images)
    {
        var categoryExists = await _context.CarCategories
            .AnyAsync(c => c.Id == dto.CategoryId);

        if (!categoryExists)
        {
            throw new InvalidOperationException(
                "The selected category does not exist.");
        }

        var car = new Car
        {
            CategoryId = dto.CategoryId,

            NameAr = dto.NameAr.Trim(),
            NameEn = dto.NameEn.Trim(),

            DescriptionAr = dto.DescriptionAr?.Trim(),
            DescriptionEn = dto.DescriptionEn?.Trim(),

            PricePerDay = dto.PricePerDay,
            PricePerWeek = dto.PricePerWeek,
            PricePerMonth = dto.PricePerMonth,

            Transmission = dto.Transmission,
            FuelType = dto.FuelType,

            Seats = dto.Seats,
            Doors = dto.Doors,
            Luggage = dto.Luggage,

            Rating = 0,
            ReviewsCount = 0,

            IsActive = true,
            IsFeatured = dto.IsFeatured,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Cars.Add(car);

        await _context.SaveChangesAsync();

        // =========================
        // Upload Images
        // =========================

        var imageList = images.ToList();

        foreach (var image in imageList)
        {
            var result = await _imageStorage.UploadAsync(
                image.Stream,
                image.FileName,
                $"mym-car-rental/cars/{car.Id}");

            car.Images.Add(new CarImage
            {
                CarId = car.Id,
                ImageUrl = result.ImageUrl,
                PublicId = result.PublicId,
                IsPrimary = false,
                SortOrder = car.Images.Count,
                CreatedAt = DateTime.UtcNow
            });
        }

        // First image becomes primary
        var firstImage = car.Images.FirstOrDefault();

        if (firstImage is not null)
        {
            firstImage.IsPrimary = true;
        }

        await _context.SaveChangesAsync();

        await _context.Entry(car)
            .Reference(c => c.Category)
            .LoadAsync();

        return MapToDto(car);
    }

    // =========================
    // Update
    // =========================

    public async Task<CarDto?> UpdateAsync(
        int id,
        UpdateCarDto dto,
        IEnumerable<(Stream Stream, string FileName)> newImages)
    {
        var car = await _context.Cars
            .Include(c => c.Images)
            .Include(c => c.Category)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (car is null)
        {
            return null;
        }

        var categoryExists = await _context.CarCategories
            .AnyAsync(c => c.Id == dto.CategoryId);

        if (!categoryExists)
        {
            throw new InvalidOperationException(
                "The selected category does not exist.");
        }

        // =========================
        // Update Data
        // =========================

        car.CategoryId = dto.CategoryId;

        car.NameAr = dto.NameAr.Trim();
        car.NameEn = dto.NameEn.Trim();

        car.DescriptionAr = dto.DescriptionAr?.Trim();
        car.DescriptionEn = dto.DescriptionEn?.Trim();

        car.PricePerDay = dto.PricePerDay;
        car.PricePerWeek = dto.PricePerWeek;
        car.PricePerMonth = dto.PricePerMonth;

        car.Transmission = dto.Transmission;
        car.FuelType = dto.FuelType;

        car.Seats = dto.Seats;
        car.Doors = dto.Doors;
        car.Luggage = dto.Luggage;

        car.IsActive = dto.IsActive;
        car.IsFeatured = dto.IsFeatured;

        car.UpdatedAt = DateTime.UtcNow;

        // =========================
        // Upload New Images
        // =========================

        var imageList = newImages.ToList();

        foreach (var image in imageList)
        {
            var result = await _imageStorage.UploadAsync(
                image.Stream,
                image.FileName,
                $"mym-car-rental/cars/{car.Id}");

            var nextSortOrder = car.Images.Any()
                ? car.Images.Max(i => i.SortOrder) + 1
                : 0;

            car.Images.Add(new CarImage
            {
                CarId = car.Id,
                ImageUrl = result.ImageUrl,
                PublicId = result.PublicId,
                IsPrimary = !car.Images.Any(),
                SortOrder = nextSortOrder,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        return MapToDto(car);
    }

    // =========================
    // Delete Car
    // =========================

    public async Task<bool> DeleteAsync(int id)
    {
        var car = await _context.Cars
            .Include(c => c.Images)
            .Include(c => c.Bookings)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (car is null)
        {
            return false;
        }

        if (car.Bookings.Any())
        {
            throw new InvalidOperationException(
                "Cannot delete a car that has bookings.");
        }

        var publicIds = car.Images
            .Select(i => i.PublicId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToList();

        _context.Cars.Remove(car);

        await _context.SaveChangesAsync();

        // Delete all images from Cloudinary
        foreach (var publicId in publicIds)
        {
            await _imageStorage.DeleteAsync(publicId);
        }

        return true;
    }

    // =========================
    // Delete Image
    // =========================

    public async Task<bool> DeleteImageAsync(
        int carId,
        int imageId)
    {
        var image = await _context.CarImages
            .FirstOrDefaultAsync(i =>
                i.Id == imageId &&
                i.CarId == carId);

        if (image is null)
        {
            return false;
        }

        // Don't allow deleting the only image
        var imageCount = await _context.CarImages
            .CountAsync(i => i.CarId == carId);

        if (imageCount == 1)
        {
            throw new InvalidOperationException(
                "A car must have at least one image.");
        }

        var publicId = image.PublicId;
        var wasPrimary = image.IsPrimary;

        _context.CarImages.Remove(image);

        if (wasPrimary)
        {
            var nextPrimary = await _context.CarImages
                .Where(i =>
                    i.CarId == carId &&
                    i.Id != imageId)
                .OrderBy(i => i.SortOrder)
                .FirstOrDefaultAsync();

            if (nextPrimary is not null)
            {
                nextPrimary.IsPrimary = true;
            }
        }

        await _context.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(publicId))
        {
            await _imageStorage.DeleteAsync(publicId);
        }

        return true;
    }

    // =========================
    // Set Primary Image
    // =========================

    public async Task<bool> SetPrimaryImageAsync(
        int carId,
        int imageId)
    {
        var images = await _context.CarImages
            .Where(i => i.CarId == carId)
            .ToListAsync();

        var targetImage = images
            .FirstOrDefault(i => i.Id == imageId);

        if (targetImage is null)
        {
            return false;
        }

        foreach (var image in images)
        {
            image.IsPrimary = image.Id == imageId;
        }

        await _context.SaveChangesAsync();

        return true;
    }

    // =========================
    // Mapping
    // =========================

    private static CarDto MapToDto(Car car)
    {
        var primaryImage = car.Images
            .FirstOrDefault(i => i.IsPrimary);

        return new CarDto
        {
            Id = car.Id,

            CategoryId = car.CategoryId,

            NameAr = car.NameAr,
            NameEn = car.NameEn,

            DescriptionAr = car.DescriptionAr,
            DescriptionEn = car.DescriptionEn,

            CategoryNameAr = car.Category.NameAr,
            CategoryNameEn = car.Category.NameEn,

            PricePerDay = car.PricePerDay,
            PricePerWeek = car.PricePerWeek,
            PricePerMonth = car.PricePerMonth,

            Transmission = car.Transmission,
            FuelType = car.FuelType,

            Seats = car.Seats,
            Doors = car.Doors,
            Luggage = car.Luggage,

            Rating = car.Rating,
            ReviewsCount = car.ReviewsCount,

            IsActive = car.IsActive,
            IsFeatured = car.IsFeatured,

            PrimaryImageUrl = primaryImage?.ImageUrl,

            Images = car.Images
                .OrderBy(i => i.SortOrder)
                .Select(i => new CarImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    IsPrimary = i.IsPrimary,
                    SortOrder = i.SortOrder
                })
                .ToList(),

            CreatedAt = car.CreatedAt,
            UpdatedAt = car.UpdatedAt
        };
    }
}