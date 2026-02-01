using MassTransit;
using Microsoft.EntityFrameworkCore;
using Trip.Domain.Entities;

namespace Trip.Infrastructure.Persistence;

public class TripDbContext : DbContext
{
    public TripDbContext(DbContextOptions<TripDbContext> options) : base(options)
    {
    }

    public DbSet<TripBooking> TripBookings => Set<TripBooking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TripBooking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerEmail).HasMaxLength(256).IsRequired();
            entity.Property(e => e.CustomerName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.OutboundFlightConfirmation).HasMaxLength(50);
            entity.Property(e => e.ReturnFlightConfirmation).HasMaxLength(50);
            entity.Property(e => e.HotelConfirmation).HasMaxLength(50);
            entity.Property(e => e.GroundTransportConfirmation).HasMaxLength(50);
            entity.Property(e => e.InsurancePolicyNumber).HasMaxLength(50);
            entity.Property(e => e.PaymentConfirmation).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.FailureReason).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasPrecision(3);
            entity.Property(e => e.CompletedAt).HasPrecision(3);
            entity.Property(e => e.CancelledAt).HasPrecision(3);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.Status);
        });

        // MassTransit Inbox/Outbox tables for message idempotency and transactional outbox
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
