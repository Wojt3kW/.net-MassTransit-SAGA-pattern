using HotelBooking.Application.Abstractions;
using HotelBooking.Contracts.Events;
using HotelBooking.Domain.Entities;
using MassTransit;
using ConfirmHotelCommand = HotelBooking.Contracts.Commands.ConfirmHotel;

namespace HotelBooking.API.Features.ConfirmHotel;

/// <summary>
/// Consumer that handles hotel confirmation commands.
/// </summary>
public class ConfirmHotelConsumer : IConsumer<ConfirmHotelCommand>
{
    private readonly IHotelReservationRepository _repository;

    public ConfirmHotelConsumer(IHotelReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<ConfirmHotelCommand> context)
    {
        var command = context.Message;

        var reservation = await _repository.GetByIdAsync(command.HotelReservationId, context.CancellationToken);

        if (reservation is null || reservation.Status != HotelReservationStatus.Reserved)
        {
            await context.Publish(new HotelReservationFailed(
                command.CorrelationId,
                command.TripId,
                "Reservation not found or already processed"));
            return;
        }

        if (reservation.ExpiresAt.HasValue && reservation.ExpiresAt < DateTime.UtcNow)
        {
            reservation.Status = HotelReservationStatus.Expired;
            await _repository.UpdateAsync(reservation, context.CancellationToken);

            await context.Publish(new HotelConfirmationExpired(
                command.CorrelationId,
                command.TripId,
                command.HotelReservationId));
            return;
        }

        // SIMULATION: If hotel name contains "CONFIRM_TIMEOUT" - simulate timeout
        if (reservation.HotelName.Contains("CONFIRM_TIMEOUT", StringComparison.OrdinalIgnoreCase))
        {
            await Task.Delay(TimeSpan.FromMinutes(16), context.CancellationToken);
        }

        reservation.Status = HotelReservationStatus.Confirmed;
        reservation.ConfirmedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(reservation, context.CancellationToken);

        await context.Publish(new HotelConfirmed(
            command.CorrelationId,
            command.TripId,
            reservation.Id,
            DateTime.UtcNow));
    }
}
