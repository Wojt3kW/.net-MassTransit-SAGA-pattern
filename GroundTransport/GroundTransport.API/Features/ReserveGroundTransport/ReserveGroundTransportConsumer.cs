using GroundTransport.Application.Abstractions;
using GroundTransport.Contracts.Events;
using GroundTransport.Domain.Entities;
using MassTransit;
using ReserveGroundTransportCommand = GroundTransport.Contracts.Commands.ReserveGroundTransport;

namespace GroundTransport.API.Features.ReserveGroundTransport;

/// <summary>
/// Consumer that handles ground transport reservation commands.
/// </summary>
public class ReserveGroundTransportConsumer : IConsumer<ReserveGroundTransportCommand>
{
    private readonly ITransportReservationRepository _repository;

    public ReserveGroundTransportConsumer(ITransportReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<ReserveGroundTransportCommand> context)
    {
        var command = context.Message;

        // SIMULATION: If type contains "FAIL" - simulate reservation failure
        if (command.Type.Contains("FAIL"))
        {
            await context.Publish(new GroundTransportReservationFailed(
                command.CorrelationId,
                command.TripId,
                "Simulated: No vehicles available"));
            return;
        }

        // SIMULATION: If type starts with "TIMEOUT" - simulate a timeout
        if (command.Type.StartsWith("TIMEOUT"))
        {
            await Task.Delay(TimeSpan.FromSeconds(65), context.CancellationToken);
        }

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

        await _repository.AddAsync(reservation, context.CancellationToken);

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
