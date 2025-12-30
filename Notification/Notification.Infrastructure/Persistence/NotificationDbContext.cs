using Microsoft.EntityFrameworkCore;
using Notification.Domain.Entities;

namespace Notification.Infrastructure.Persistence;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    public DbSet<NotificationRecord> Notifications => Set<NotificationRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Recipient).HasMaxLength(256).IsRequired();
            entity.Property(e => e.Subject).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Body).IsRequired();
            entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(30);
            entity.Property(e => e.Channel).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.ErrorMessage).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasPrecision(3);
            entity.Property(e => e.SentAt).HasPrecision(3);
            
            entity.HasIndex(e => e.TripId);
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.Status);
        });
    }
}
