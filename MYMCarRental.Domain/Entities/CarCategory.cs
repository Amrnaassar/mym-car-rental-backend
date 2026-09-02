namespace MYMCarRental.Domain.Entities;

public class CarCategory
{
    public int Id { get; set; }

    // =========================
    // Multilingual Content
    // =========================

    public string NameAr { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public string? DescriptionAr { get; set; }

    public string? DescriptionEn { get; set; }

    // =========================
    // Identification
    // =========================

    public string Slug { get; set; } = string.Empty;

    // =========================
    // Image
    // =========================

    public string? ImageUrl { get; set; }

    public string? ImagePublicId { get; set; }

    // =========================
    // Status
    // =========================

    public bool IsActive { get; set; } = true;

    // =========================
    // Audit
    // =========================

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // =========================
    // Relationships
    // =========================

    public ICollection<Car> Cars { get; set; } = new List<Car>();
}