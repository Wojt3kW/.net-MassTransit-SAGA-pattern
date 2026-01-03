using HotelBooking.Application.Abstractions;
using HotelBooking.Contracts.Events;
using HotelBooking.Domain.Entities;
using MassTransit;
using CancelHotelCommand = HotelBooking.Contracts.Commands.CancelHotel;

namespace HotelBooking.API.Features.CancelHotel;

/// <summary>
/// Consumer that handles hotel cancellation commands.
/// </summary>
public class CancelHotelConsumer : IConsumer<CancelHotelCommand>
{
    private readonly IHotelReservationRepository _repository;

    public CancelHotelConsumer(IHotelReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<CancelHotelCommand> context)
    {
        var command = context.Message;

        var reservation = await _repository.GetByIdAsync(command.HotelReservationId, context.CancellationToken);

        var utcNow = DateTime.UtcNow;

        if (reservation is not null)
        {
            reservation.Status = HotelReservationStatus.Cancelled;
            reservation.CancellationReason = command.Reason;
            reservation.CancelledAt = utcNow;

            await _repository.UpdateAsync(reservation, context.CancellationToken);
        }

        await context.Publish(new HotelCancelled(
            command.CorrelationId,
            command.TripId,
            command.HotelReservationId,
            utcNow,
            command.Reason));
    }
}
