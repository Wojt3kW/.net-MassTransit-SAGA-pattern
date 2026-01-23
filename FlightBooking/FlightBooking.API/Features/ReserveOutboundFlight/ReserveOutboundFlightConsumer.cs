using FlightBooking.Application.Abstractions;
using FlightBooking.Contracts.Events;
using FlightBooking.Domain.Entities;
using MassTransit;
using ReserveOutboundFlightCommand = FlightBooking.Contracts.Commands.ReserveOutboundFlight;

namespace FlightBooking.API.Features.ReserveOutboundFlight;

/// <summary>
/// Consumer that handles outbound flight reservation commands.
/// </summary>
public class ReserveOutboundFlightConsumer : IConsumer<ReserveOutboundFlightCommand>
{
    private readonly IFlightReservationRepository _repository;

    public ReserveOutboundFlightConsumer(IFlightReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<ReserveOutboundFlightCommand> context)
    {
        var command = context.Message;

        // SIMULATION: If the flight number contains "FAIL" - simulate an error (before saving)
        if (command.FlightNumber.Contains("FAIL"))
        {
            await context.Publish(new FlightReservationFailed(
                CorrelationId: command.CorrelationId,
                TripId: command.TripId,
                FlightNumber: command.FlightNumber,
                Reason: "Simulated: Outbound flight unavailable"));
            return;
        }

        // SIMULATION: If the flight number starts with "TIMEOUT" - simulate a timeout
        if (command.FlightNumber.StartsWith("TIMEOUT"))
        {
            await Task.Delay(TimeSpan.FromSeconds(65), context.CancellationToken);
        }

        // Simulate flight reservation (in real scenario, call external airline API)
        var reservation = new FlightReservation
        {
            Id = Guid.NewGuid(),
            TripId = command.TripId,
            Type = FlightType.Outbound,
            FlightNumber = command.FlightNumber,
            Carrier = command.Carrier,
            Origin = command.Origin,
            Destination = command.Destination,
            DepartureDate = command.DepartureDate,
            PassengerCount = command.PassengerCount,
            ConfirmationCode = GenerateConfirmationCode(),
            Price = CalculatePrice(command),
            Status = ReservationStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            ConfirmedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(reservation, context.CancellationToken);

        await context.Publish(new OutboundFlightReserved(
            command.CorrelationId,
            command.TripId,
            reservation.Id,
            reservation.FlightNumber,
            reservation.ConfirmationCode!,
            reservation.DepartureDate,
            reservation.Price));
    }

    private static string GenerateConfirmationCode() => $"FL-{Guid.NewGuid().ToString()[..8].ToUpper()}";

    private static decimal CalculatePrice(ReserveOutboundFlightCommand command) => 250.00m * command.PassengerCount;
}
