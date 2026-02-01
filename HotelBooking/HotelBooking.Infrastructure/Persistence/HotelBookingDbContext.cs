using HotelBooking.Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence;

public class HotelBookingDbContext : DbContext
{
    public HotelBookingDbContext(DbContextOptions<HotelBookingDbContext> options) : base(options)
    {
    }

    public DbSet<HotelReservation> HotelReservations => Set<HotelReservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HotelReservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HotelName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.GuestName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.GuestEmail).HasMaxLength(256).IsRequired();
            entity.Property(e => e.ConfirmationCode).HasMaxLength(50);
            entity.Property(e => e.PricePerNight).HasPrecision(18, 2);
            entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.CancellationReason).HasMaxLength(500);
            entity.Property(e => e.CheckIn).HasPrecision(3);
            entity.Property(e => e.CheckOut).HasPrecision(3);
            entity.Property(e => e.CreatedAt).HasPrecision(3);
            entity.Property(e => e.ConfirmedAt).HasPrecision(3);
            entity.Property(e => e.CancelledAt).HasPrecision(3);
            entity.Property(e => e.ExpiresAt).HasPrecision(3);

            entity.HasIndex(e => e.TripId);
            entity.HasIndex(e => e.HotelId);
            entity.HasIndex(e => e.Status);
        });

        // MassTransit Inbox/Outbox tables for message idempotency and transactional outbox
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
