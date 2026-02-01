using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;

namespace Payment.Infrastructure.Persistence;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CardLastFourDigits).HasMaxLength(4).IsRequired();
            entity.Property(e => e.CardHolderName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Currency).HasMaxLength(3).IsRequired();
            entity.Property(e => e.AuthorisationCode).HasMaxLength(50);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.FailureReason).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasPrecision(3);
            entity.Property(e => e.AuthorisedAt).HasPrecision(3);
            entity.Property(e => e.CapturedAt).HasPrecision(3);
            entity.Property(e => e.ReleasedAt).HasPrecision(3);
            entity.Property(e => e.RefundedAt).HasPrecision(3);
            
            entity.HasIndex(e => e.TripId);
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.Status);
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.CardLastFourDigits).HasMaxLength(4).IsRequired();
            entity.Property(e => e.CardNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.CardHolderName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.ExpiryDate).HasMaxLength(5).IsRequired();
            entity.Property(e => e.Cvv).HasMaxLength(4).IsRequired();
            entity.Property(e => e.CreatedAt).HasPrecision(3);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => new { e.CustomerId, e.IsDefault });
        });

        // MassTransit Inbox/Outbox tables for message idempotency and transactional outbox
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
