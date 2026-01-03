using FlightBooking.Application.Abstractions;
using FlightBooking.Contracts.Events;
using FlightBooking.Domain.Entities;
using MassTransit;
using CancelFlightCommand = FlightBooking.Contracts.Commands.CancelFlight;

namespace FlightBooking.API.Features.CancelFlight;

/// <summary>
/// Consumer that handles flight cancellation commands.
/// </summary>
public class CancelFlightConsumer : IConsumer<CancelFlightCommand>
{
    private readonly IFlightReservationRepository _repository;

    public CancelFlightConsumer(IFlightReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<CancelFlightCommand> context)
    {
        var command = context.Message;

        var reservation = await _repository.GetByIdAsync(command.FlightReservationId, context.CancellationToken);

        var utcNow = DateTime.UtcNow;

        if (reservation is not null)
        {
            reservation.Status = ReservationStatus.Cancelled;
            reservation.CancellationReason = command.Reason;
            reservation.CancelledAt = utcNow;

            await _repository.UpdateAsync(reservation, context.CancellationToken);
        }

        await context.Publish(new FlightCancelled(
            command.CorrelationId,
            command.TripId,
            command.FlightReservationId,
            utcNow,
            command.Reason));
    }
}
