using FlightBooking.Domain.Entities;
using FlightBooking.Infrastructure.Persistence;
using FlightBooking.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using CancelFlightCommand = FlightBooking.Contracts.Commands.CancelFlight;

namespace FlightBooking.API.Features.CancelFlight;

public class CancelFlightConsumer : IConsumer<CancelFlightCommand>
{
    private readonly FlightBookingDbContext _dbContext;

    public CancelFlightConsumer(FlightBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CancelFlightCommand> context)
    {
        var command = context.Message;

        var reservation = await _dbContext.FlightReservations
            .FirstOrDefaultAsync(x => x.Id == command.FlightReservationId);

        if (reservation is not null)
        {
            reservation.Status = ReservationStatus.Cancelled;
            reservation.CancellationReason = command.Reason;
            reservation.CancelledAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();
        }

        await context.Publish(new FlightCancelled(
            command.CorrelationId,
            command.TripId,
            command.FlightReservationId,
            DateTime.UtcNow));
    }
}
