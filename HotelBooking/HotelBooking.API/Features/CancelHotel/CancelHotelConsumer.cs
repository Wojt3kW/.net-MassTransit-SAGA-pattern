using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using CancelHotelCommand = HotelBooking.Contracts.Commands.CancelHotel;

namespace HotelBooking.API.Features.CancelHotel;

public class CancelHotelConsumer : IConsumer<CancelHotelCommand>
{
    private readonly HotelBookingDbContext _dbContext;

    public CancelHotelConsumer(HotelBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CancelHotelCommand> context)
    {
        var command = context.Message;

        var reservation = await _dbContext.HotelReservations
            .FirstOrDefaultAsync(x => x.Id == command.HotelReservationId);

        if (reservation is not null)
        {
            reservation.Status = HotelReservationStatus.Cancelled;
            reservation.CancellationReason = command.Reason;
            reservation.CancelledAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();
        }

        await context.Publish(new HotelCancelled(
            command.CorrelationId,
            command.TripId,
            command.HotelReservationId,
            DateTime.UtcNow));
    }
}
