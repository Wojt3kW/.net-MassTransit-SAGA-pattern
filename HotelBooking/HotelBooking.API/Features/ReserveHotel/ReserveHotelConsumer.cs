using HotelBooking.Application.Abstractions;
using HotelBooking.Contracts.Events;
using HotelBooking.Domain.Entities;
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

        // SIMULATION: If hotel name contains "FAIL" - simulate reservation failure
        if (command.HotelName.Contains("FAIL"))
        {
            await context.Publish(new HotelReservationFailed(
                command.CorrelationId,
                command.TripId,
                $"Simulated error: No hotel with name {command.HotelName}"));
            return;
        }

        // SIMULATION: If hotel name starts with "TIMEOUT" - simulate a timeout
        if (command.HotelName.StartsWith("TIMEOUT"))
        {
            await Task.Delay(TimeSpan.FromSeconds(65), context.CancellationToken);
        }

        var nights = (command.CheckOut - command.CheckIn).Days;

        // Simulate hotel reservation (in real scenario, call external API)
        await Task.Delay(TimeSpan.FromSeconds(10));

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
