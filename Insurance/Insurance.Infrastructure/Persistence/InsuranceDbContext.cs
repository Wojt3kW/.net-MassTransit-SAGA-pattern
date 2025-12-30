using Insurance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Infrastructure.Persistence;

public class InsuranceDbContext : DbContext
{
    public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options) : base(options)
    {
    }

    public DbSet<InsurancePolicy> InsurancePolicies => Set<InsurancePolicy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InsurancePolicy>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.CustomerEmail).HasMaxLength(256).IsRequired();
            entity.Property(e => e.PolicyNumber).HasMaxLength(50);
            entity.Property(e => e.TripTotalValue).HasPrecision(18, 2);
            entity.Property(e => e.Premium).HasPrecision(18, 2);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.CancellationReason).HasMaxLength(500);
            entity.Property(e => e.CoverageStartDate).HasPrecision(3);
            entity.Property(e => e.CoverageEndDate).HasPrecision(3);
            entity.Property(e => e.CreatedAt).HasPrecision(3);
            entity.Property(e => e.IssuedAt).HasPrecision(3);
            entity.Property(e => e.CancelledAt).HasPrecision(3);
            
            entity.HasIndex(e => e.TripId);
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.PolicyNumber);
            entity.HasIndex(e => e.Status);
        });
    }
}
