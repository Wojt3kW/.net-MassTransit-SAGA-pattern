using FlightBooking.Application.Abstractions;
using FlightBooking.Domain.Entities;
using FlightBooking.Contracts.Events;
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
