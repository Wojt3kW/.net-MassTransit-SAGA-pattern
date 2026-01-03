using GroundTransport.Application.Abstractions;
using GroundTransport.Contracts.Events;
using GroundTransport.Domain.Entities;
using MassTransit;
using CancelGroundTransportCommand = GroundTransport.Contracts.Commands.CancelGroundTransport;

namespace GroundTransport.API.Features.CancelGroundTransport;

/// <summary>
/// Consumer that handles ground transport cancellation commands.
/// </summary>
public class CancelGroundTransportConsumer : IConsumer<CancelGroundTransportCommand>
{
    private readonly ITransportReservationRepository _repository;

    public CancelGroundTransportConsumer(ITransportReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<CancelGroundTransportCommand> context)
    {
        var command = context.Message;

        var reservation = await _repository.GetByIdAsync(command.TransportReservationId, context.CancellationToken);

        var utcNow = DateTime.UtcNow;

        if (reservation is not null)
        {
            reservation.Status = TransportReservationStatus.Cancelled;
            reservation.CancellationReason = command.Reason;
            reservation.CancelledAt = utcNow;

            await _repository.UpdateAsync(reservation, context.CancellationToken);
        }

        await context.Publish(new GroundTransportCancelled(
            command.CorrelationId,
            command.TripId,
            command.TransportReservationId,
            utcNow,
            command.Reason));
    }
}
