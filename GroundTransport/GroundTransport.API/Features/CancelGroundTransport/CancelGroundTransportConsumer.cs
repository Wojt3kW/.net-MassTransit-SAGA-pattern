using GroundTransport.Domain.Entities;
using GroundTransport.Infrastructure.Persistence;
using GroundTransport.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using CancelGroundTransportCommand = GroundTransport.Contracts.Commands.CancelGroundTransport;

namespace GroundTransport.API.Features.CancelGroundTransport;

public class CancelGroundTransportConsumer : IConsumer<CancelGroundTransportCommand>
{
    private readonly GroundTransportDbContext _dbContext;

    public CancelGroundTransportConsumer(GroundTransportDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CancelGroundTransportCommand> context)
    {
        var command = context.Message;

        var reservation = await _dbContext.TransportReservations
            .FirstOrDefaultAsync(x => x.Id == command.TransportReservationId);

        if (reservation is not null)
        {
            reservation.Status = TransportReservationStatus.Cancelled;
            reservation.CancellationReason = command.Reason;
            reservation.CancelledAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();
        }

        await context.Publish(new GroundTransportCancelled(
            command.CorrelationId,
            command.TripId,
            command.TransportReservationId,
            DateTime.UtcNow));
    }
}
