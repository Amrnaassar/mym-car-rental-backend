namespace MYMCarRental.Domain.Entities;

public class CarImage
{
    public int Id { get; set; }

    public int CarId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string PublicId { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public Car Car { get; set; } = null!;
}