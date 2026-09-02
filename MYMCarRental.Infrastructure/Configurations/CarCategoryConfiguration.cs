using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MYMCarRental.Domain.Entities;

namespace MYMCarRental.Infrastructure.Configurations;

public class CarCategoryConfiguration : IEntityTypeConfiguration<CarCategory>
{
    public void Configure(EntityTypeBuilder<CarCategory> builder)
    {
        builder.ToTable("CarCategories");

        builder.HasKey(c => c.Id);

        // =========================
        // Multilingual Content
        // =========================

        builder.Property(c => c.NameAr)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.NameEn)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.DescriptionAr)
            .HasMaxLength(500);

        builder.Property(c => c.DescriptionEn)
            .HasMaxLength(500);

        // =========================
        // Identification
        // =========================

        builder.Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(c => c.Slug)
            .IsUnique();

        // =========================
        // Image
        // =========================

        builder.Property(c => c.ImageUrl)
            .HasMaxLength(500);

        builder.Property(c => c.ImagePublicId)
            .HasMaxLength(255);

        // =========================
        // Status
        // =========================

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        // =========================
        // Audit
        // =========================

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired();

        // =========================
        // Relationships
        // =========================

        builder.HasMany(c => c.Cars)
            .WithOne(c => c.Category)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}