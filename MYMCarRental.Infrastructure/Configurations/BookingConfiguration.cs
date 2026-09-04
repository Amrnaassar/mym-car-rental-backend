using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MYMCarRental.Domain.Entities;

namespace MYMCarRental.Infrastructure.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .ValueGeneratedNever();

        builder.Property(b => b.BookingNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasIndex(b => b.BookingNumber)
            .IsUnique();

        builder.Property(b => b.PickupLocation)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.CustomerFullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(b => b.CustomerEmail)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(b => b.CustomerPhone)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(b => b.DrivingLicense)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Notes)
            .HasMaxLength(1000);

        builder.Property(b => b.DailyRateSnapshot)
            .HasPrecision(18, 2);

        builder.Property(b => b.WeeklyRateSnapshot)
            .HasPrecision(18, 2);

        builder.Property(b => b.MonthlyRateSnapshot)
            .HasPrecision(18, 2);

        builder.Property(b => b.RentalCost)
            .HasPrecision(18, 2);

        builder.Property(b => b.InsuranceCost)
            .HasPrecision(18, 2);

        builder.Property(b => b.TaxCost)
            .HasPrecision(18, 2);

        builder.Property(b => b.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(b => b.GrandTotal)
            .HasPrecision(18, 2);

        builder.Property(b => b.Status)
            .IsRequired();

        builder.Property(b => b.RentalPlan)
            .IsRequired();

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.UpdatedAt)
            .IsRequired();

        builder.HasIndex(b => new
        {
            b.CarId,
            b.PickupDate,
            b.ReturnDate
        });

        builder.HasIndex(b => b.Status);

        builder.HasIndex(b => b.CustomerEmail);

        builder.HasOne(b => b.Car)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.CarId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}