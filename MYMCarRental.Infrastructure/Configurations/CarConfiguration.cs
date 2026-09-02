using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MYMCarRental.Domain.Entities;

namespace MYMCarRental.Infrastructure.Configurations;

public class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.ToTable("Cars");

        builder.HasKey(c => c.Id);

        // =========================
        // Multilingual Content
        // =========================

        builder.Property(c => c.NameAr)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.NameEn)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.DescriptionAr)
            .HasMaxLength(1000);

        builder.Property(c => c.DescriptionEn)
            .HasMaxLength(1000);

        // =========================
        // Pricing
        // =========================

        builder.Property(c => c.PricePerDay)
            .HasPrecision(18, 2);

        builder.Property(c => c.PricePerWeek)
            .HasPrecision(18, 2);

        builder.Property(c => c.PricePerMonth)
            .HasPrecision(18, 2);

        // =========================
        // Rating
        // =========================

        builder.Property(c => c.Rating)
            .HasPrecision(3, 2);

        // =========================
        // Specifications
        // =========================

        builder.Property(c => c.Transmission)
            .IsRequired();

        builder.Property(c => c.FuelType)
            .IsRequired();

        // =========================
        // Status
        // =========================

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.Property(c => c.IsFeatured)
            .HasDefaultValue(false);

        // =========================
        // Audit
        // =========================

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired();

        // =========================
        // Indexes
        // =========================

        builder.HasIndex(c => c.CategoryId);

        builder.HasIndex(c => new
        {
            c.IsActive,
            c.IsFeatured
        });

        // =========================
        // Relationships
        // =========================

        builder.HasOne(c => c.Category)
            .WithMany(c => c.Cars)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Images)
            .WithOne(i => i.Car)
            .HasForeignKey(i => i.CarId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Bookings)
            .WithOne(b => b.Car)
            .HasForeignKey(b => b.CarId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}