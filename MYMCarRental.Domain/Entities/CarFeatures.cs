
namespace MYMCarRental.Domain.Entities;

public class CarFeatures
{
    public int Id { get; set; }

    public int CarId { get; set; }

    public string featureAr { get; set; } = string.Empty;

    public string featureEn { get; set; } = string.Empty;

    public Car Car { get; set; } = null!;

}

