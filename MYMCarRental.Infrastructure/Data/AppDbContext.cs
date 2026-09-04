using Microsoft.EntityFrameworkCore;
using MYMCarRental.Domain.Entities;

namespace MYMCarRental.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<CarCategory> CarCategories => Set<CarCategory>();

    public DbSet<Car> Cars => Set<Car>();

    public DbSet<CarImage> CarImages => Set<CarImage>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly
        );
    }
}