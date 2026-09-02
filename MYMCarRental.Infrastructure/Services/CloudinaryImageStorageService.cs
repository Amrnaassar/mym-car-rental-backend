using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MYMCarRental.Application.Interfaces;

namespace MYMCarRental.Infrastructure.Services;

public class CloudinaryImageStorageService : IImageStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryImageStorageService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<(string ImageUrl, string PublicId)> UploadAsync(
        Stream fileStream,
        string fileName,
        string folder)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = folder,
            UseFilename = true,
            UniqueFilename = true
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
        {
            throw new InvalidOperationException(
                $"Cloudinary upload failed: {result.Error.Message}");
        }

        return (
            result.SecureUrl.ToString(),
            result.PublicId
        );
    }

    public async Task DeleteAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);

        var result = await _cloudinary.DestroyAsync(deleteParams);

        if (result.Error != null)
        {
            throw new InvalidOperationException(
                $"Cloudinary delete failed: {result.Error.Message}");
        }
    }
}