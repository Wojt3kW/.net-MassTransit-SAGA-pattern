using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripBooking.Saga.States;

namespace TripBooking.Saga.Persistence
{
    /// <summary>
    /// Entity Framework Core mapping configuration for TripBookingSagaState.
    /// </summary>
    public class TripBookingSagaStateMap : SagaClassMap<TripBookingSagaState>
    {
        protected override void Configure(EntityTypeBuilder<TripBookingSagaState> entity, ModelBuilder model)
        {
            // Primary key
            entity.HasKey(e => e.CorrelationId);

            // Required string properties with max length
            entity.Property(e => e.CurrentState)
                .IsRequired()
                .HasMaxLength(64);

            entity.Property(e => e.CustomerEmail)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(e => e.CustomerName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Origin)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Destination)
                .IsRequired()
                .HasMaxLength(100);

            // Flight details
            entity.Property(e => e.OutboundFlightNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.OutboundCarrier)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.ReturnFlightNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.ReturnCarrier)
                .IsRequired()
                .HasMaxLength(100);

            // Hotel details
            entity.Property(e => e.HotelId)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.HotelName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.CheckIn)
                .HasPrecision(3);

            entity.Property(e => e.CheckOut)
                .HasPrecision(3);

            // Ground transport details (optional)
            entity.Property(e => e.GroundTransportType)
                .HasMaxLength(50);

            entity.Property(e => e.GroundTransportPickupLocation)
                .HasMaxLength(200);

            entity.Property(e => e.GroundTransportDropoffLocation)
                .HasMaxLength(200);

            // Currency
            entity.Property(e => e.Currency)
                .IsRequired()
                .HasMaxLength(3);

            // Optional string properties
            entity.Property(e => e.FailureReason)
                .HasMaxLength(500);

            // Decimal precision for money
            entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 2);

            // DateTime precision (milliseconds)
            entity.Property(e => e.DepartureDate)
                .HasPrecision(3);

            entity.Property(e => e.ReturnDate)
                .HasPrecision(3);

            entity.Property(e => e.CreatedAt)
                .HasPrecision(3);

            entity.Property(e => e.CompletedAt)
                .HasPrecision(3);

            // Indexes for query performance
            entity.HasIndex(e => e.TripId);
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.CurrentState);
        }
    }
}
