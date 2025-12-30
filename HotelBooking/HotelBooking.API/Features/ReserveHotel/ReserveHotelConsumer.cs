using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Contracts.Events;
using MassTransit;
using ReserveHotelCommand = HotelBooking.Contracts.Commands.ReserveHotel;

namespace HotelBooking.API.Features.ReserveHotel;

public class ReserveHotelConsumer : IConsumer<ReserveHotelCommand>
{
    private readonly HotelBookingDbContext _dbContext;
    private readonly decimal _defaultPricePerNight = 150.00m;

    public ReserveHotelConsumer(HotelBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ReserveHotelCommand> context)
    {
        var command = context.Message;

        var nights = (command.CheckOut - command.CheckIn).Days;
        
        var reservation = new HotelReservation
        {
            Id = Guid.NewGuid(),
            TripId = command.TripId,
            HotelId = command.HotelId,
            HotelName = command.HotelName,
            CheckIn = command.CheckIn,
            CheckOut = command.CheckOut,
            NumberOfGuests = command.NumberOfGuests,
            GuestName = command.GuestName,
            GuestEmail = command.GuestEmail,
            ConfirmationCode = GenerateConfirmationCode(),
            PricePerNight = _defaultPricePerNight,
            TotalPrice = _defaultPricePerNight * nights,
            Status = HotelReservationStatus.Reserved,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };

        _dbContext.HotelReservations.Add(reservation);
        await _dbContext.SaveChangesAsync();

        await context.Publish(new HotelReserved(
            command.CorrelationId,
            command.TripId,
            reservation.Id,
            reservation.HotelName,
            reservation.ConfirmationCode!,
            reservation.CheckIn,
            reservation.CheckOut,
            reservation.PricePerNight,
            reservation.TotalPrice));
    }

    private static string GenerateConfirmationCode() => $"HT-{Guid.NewGuid().ToString()[..8].ToUpper()}";
}
