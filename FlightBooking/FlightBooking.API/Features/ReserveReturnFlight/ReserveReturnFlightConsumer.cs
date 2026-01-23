using FlightBooking.Application.Abstractions;
using FlightBooking.Contracts.Events;
using FlightBooking.Domain.Entities;
using MassTransit;
using ReserveReturnFlightCommand = FlightBooking.Contracts.Commands.ReserveReturnFlight;

namespace FlightBooking.API.Features.ReserveReturnFlight;

/// <summary>
/// Consumer that handles return flight reservation commands.
/// </summary>
public class ReserveReturnFlightConsumer : IConsumer<ReserveReturnFlightCommand>
{
    private readonly IFlightReservationRepository _repository;

    public ReserveReturnFlightConsumer(IFlightReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<ReserveReturnFlightCommand> context)
    {
        var command = context.Message;

        // SIMULATION: If the flight number contains "FAIL" - simulate an error (before saving)
        if (command.FlightNumber.Contains("FAIL"))
        {
            await context.Publish(new FlightReservationFailed(
                CorrelationId: command.CorrelationId,
                TripId: command.TripId,
                FlightNumber: command.FlightNumber,
                Reason: "Simulated: Return flight unavailable"));
            return;
        }

        // SIMULATION: If the flight number starts with "TIMEOUT" - simulate a timeout
        if (command.FlightNumber.StartsWith("TIMEOUT"))
        {
            await Task.Delay(TimeSpan.FromSeconds(65), context.CancellationToken);
        }

        var reservation = new FlightReservation
        {
            Id = Guid.NewGuid(),
            TripId = command.TripId,
            Type = FlightType.Return,
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

        await context.Publish(new ReturnFlightReserved(
            command.CorrelationId,
            command.TripId,
            reservation.Id,
            reservation.FlightNumber,
            reservation.ConfirmationCode!,
            reservation.DepartureDate,
            reservation.Price));
    }

    private static string GenerateConfirmationCode() => $"FL-{Guid.NewGuid().ToString()[..8].ToUpper()}";

    private static decimal CalculatePrice(ReserveReturnFlightCommand command) => 250.00m * command.PassengerCount;
}
