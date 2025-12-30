using GroundTransport.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GroundTransport.Infrastructure.Persistence;

public class GroundTransportDbContext : DbContext
{
    public GroundTransportDbContext(DbContextOptions<GroundTransportDbContext> options) : base(options)
    {
    }

    public DbSet<TransportReservation> TransportReservations => Set<TransportReservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransportReservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PickupLocation).HasMaxLength(200).IsRequired();
            entity.Property(e => e.DropoffLocation).HasMaxLength(200).IsRequired();
            entity.Property(e => e.ConfirmationCode).HasMaxLength(50);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.CancellationReason).HasMaxLength(500);
            entity.Property(e => e.PickupDate).HasPrecision(3);
            entity.Property(e => e.CreatedAt).HasPrecision(3);
            entity.Property(e => e.ConfirmedAt).HasPrecision(3);
            entity.Property(e => e.CancelledAt).HasPrecision(3);
            
            entity.HasIndex(e => e.TripId);
            entity.HasIndex(e => e.Status);
        });
    }
}
