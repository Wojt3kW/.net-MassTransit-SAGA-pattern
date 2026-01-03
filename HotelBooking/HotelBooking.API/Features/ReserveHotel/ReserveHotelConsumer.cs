using HotelBooking.Application.Abstractions;
using HotelBooking.Domain.Entities;
using HotelBooking.Contracts.Events;
using MassTransit;
using ReserveHotelCommand = HotelBooking.Contracts.Commands.ReserveHotel;

namespace HotelBooking.API.Features.ReserveHotel;

/// <summary>
/// Consumer that handles hotel reservation commands.
/// </summary>
public class ReserveHotelConsumer : IConsumer<ReserveHotelCommand>
{
    private readonly IHotelReservationRepository _repository;
    private readonly decimal _defaultPricePerNight = 150.00m;

    public ReserveHotelConsumer(IHotelReservationRepository repository)
    {
        _repository = repository;
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

        await _repository.AddAsync(reservation, context.CancellationToken);

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
