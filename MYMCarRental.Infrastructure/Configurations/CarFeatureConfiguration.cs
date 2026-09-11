using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MYMCarRental.Domain.Entities;

namespace MYMCarRental.Infrastructure.Configurations;

public class CarFeatureConfiguration : IEntityTypeConfiguration<CarFeatures>
{
    public void Configure(EntityTypeBuilder<CarFeatures> builder)
    {
        builder.ToTable("CarFeatures");

        builder.HasKey(i => i.Id);
        builder.Property(c => c.featureAr)
            .HasMaxLength(500);

        builder.Property(c => c.featureEn)
            .HasMaxLength(500);

        builder.HasOne(i => i.Car)
            .WithMany(c => c.Features)
            .HasForeignKey(i => i.CarId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}