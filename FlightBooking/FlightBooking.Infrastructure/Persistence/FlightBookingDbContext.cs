using FlightBooking.Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Infrastructure.Persistence;

public class FlightBookingDbContext : DbContext
{
    public FlightBookingDbContext(DbContextOptions<FlightBookingDbContext> options) : base(options)
    {
    }

    public DbSet<FlightReservation> FlightReservations => Set<FlightReservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FlightReservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FlightNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Carrier).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Origin).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Destination).HasMaxLength(10).IsRequired();
            entity.Property(e => e.ConfirmationCode).HasMaxLength(50);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.CancellationReason).HasMaxLength(500);
            entity.Property(e => e.DepartureDate).HasPrecision(3);
            entity.Property(e => e.CreatedAt).HasPrecision(3);
            entity.Property(e => e.ConfirmedAt).HasPrecision(3);
            entity.Property(e => e.CancelledAt).HasPrecision(3);

            entity.HasIndex(e => e.TripId);
            entity.HasIndex(e => e.FlightNumber);
            entity.HasIndex(e => e.Status);
        });

        // MassTransit Inbox/Outbox tables for message idempotency and transactional outbox
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
