using GroundTransport.Domain.Entities;
using GroundTransport.Infrastructure.Persistence;
using GroundTransport.Contracts.Events;
using MassTransit;
using ReserveGroundTransportCommand = GroundTransport.Contracts.Commands.ReserveGroundTransport;

namespace GroundTransport.API.Features.ReserveGroundTransport;

public class ReserveGroundTransportConsumer : IConsumer<ReserveGroundTransportCommand>
{
    private readonly GroundTransportDbContext _dbContext;

    public ReserveGroundTransportConsumer(GroundTransportDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ReserveGroundTransportCommand> context)
    {
        var command = context.Message;

        var transportType = command.Type.ToLower() switch
        {
            "airporttransfer" => TransportType.AirportTransfer,
            "carrental" => TransportType.CarRental,
            _ => TransportType.AirportTransfer
        };

        var reservation = new TransportReservation
        {
            Id = Guid.NewGuid(),
            TripId = command.TripId,
            Type = transportType,
            PickupLocation = command.PickupLocation,
            DropoffLocation = command.DropoffLocation,
            PickupDate = command.PickupDate,
            PassengerCount = command.PassengerCount,
            ConfirmationCode = GenerateConfirmationCode(),
            Price = CalculatePrice(transportType, command.PassengerCount),
            Status = TransportReservationStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            ConfirmedAt = DateTime.UtcNow
        };

        _dbContext.TransportReservations.Add(reservation);
        await _dbContext.SaveChangesAsync();

        await context.Publish(new GroundTransportReserved(
            command.CorrelationId,
            command.TripId,
            reservation.Id,
            command.Type,
            reservation.ConfirmationCode!,
            reservation.PickupDate,
            reservation.Price));
    }

    private static string GenerateConfirmationCode() => $"GT-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    
    private static decimal CalculatePrice(TransportType type, int passengers) => type switch
    {
        TransportType.AirportTransfer => 50.00m * passengers,
        TransportType.CarRental => 75.00m,
        _ => 50.00m
    };
}
