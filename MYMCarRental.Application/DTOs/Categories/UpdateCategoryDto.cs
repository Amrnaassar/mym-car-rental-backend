using System.ComponentModel.DataAnnotations;

namespace MYMCarRental.Application.DTOs.Categories;

public class UpdateCategoryDto
{
    [Required]
    [MaxLength(100)]
    public string NameAr { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string NameEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? DescriptionAr { get; set; }

    [MaxLength(500)]
    public string? DescriptionEn { get; set; }
}