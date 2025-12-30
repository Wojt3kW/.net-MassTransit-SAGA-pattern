using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ConfirmHotelCommand = HotelBooking.Contracts.Commands.ConfirmHotel;

namespace HotelBooking.API.Features.ConfirmHotel;

public class ConfirmHotelConsumer : IConsumer<ConfirmHotelCommand>
{
    private readonly HotelBookingDbContext _dbContext;

    public ConfirmHotelConsumer(HotelBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ConfirmHotelCommand> context)
    {
        var command = context.Message;

        var reservation = await _dbContext.HotelReservations
            .FirstOrDefaultAsync(x => x.Id == command.HotelReservationId);

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
            await _dbContext.SaveChangesAsync();
            
            await context.Publish(new HotelConfirmationExpired(
                command.CorrelationId,
                command.TripId,
                command.HotelReservationId));
            return;
        }

        reservation.Status = HotelReservationStatus.Confirmed;
        reservation.ConfirmedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        await context.Publish(new HotelConfirmed(
            command.CorrelationId,
            command.TripId,
            reservation.Id,
            DateTime.UtcNow));
    }
}
