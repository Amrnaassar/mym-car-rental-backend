namespace MYMCarRental.Application.Interfaces;

public interface IImageStorageService
{
    Task<(string ImageUrl, string PublicId)> UploadAsync(
        Stream fileStream,
        string fileName,
        string folder);
    string GetOptimizedUrl( string publicId,int width = 800,int? height = null);

    Task DeleteAsync(string publicId);
}