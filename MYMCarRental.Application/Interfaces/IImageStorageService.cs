namespace MYMCarRental.Application.Interfaces;

public interface IImageStorageService
{
    Task<(string ImageUrl, string PublicId)> UploadAsync(
        Stream fileStream,
        string fileName,
        string folder);

    Task DeleteAsync(string publicId);
}