using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MYMCarRental.Domain.Entities;

namespace MYMCarRental.Infrastructure.Configurations;

public class CarImageConfiguration : IEntityTypeConfiguration<CarImage>
{
    public void Configure(EntityTypeBuilder<CarImage> builder)
    {
        builder.ToTable("CarImages");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ImageUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(i => i.PublicId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(i => i.IsPrimary)
            .HasDefaultValue(false);

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.HasIndex(i => i.CarId);

        builder.HasIndex(i => i.PublicId)
            .IsUnique();

        builder.HasOne(i => i.Car)
            .WithMany(c => c.Images)
            .HasForeignKey(i => i.CarId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}