using FlightBooking.Domain.Entities;
using FlightBooking.Infrastructure.Persistence;
using FlightBooking.Contracts.Events;
using MassTransit;
using ReserveReturnFlightCommand = FlightBooking.Contracts.Commands.ReserveReturnFlight;

namespace FlightBooking.API.Features.ReserveReturnFlight;

public class ReserveReturnFlightConsumer : IConsumer<ReserveReturnFlightCommand>
{
    private readonly FlightBookingDbContext _dbContext;

    public ReserveReturnFlightConsumer(FlightBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ReserveReturnFlightCommand> context)
    {
        var command = context.Message;

        var reservation = new FlightReservation
        {
            Id = Guid.NewGuid(),
            TripId = command.TripId,
            Type = FlightType.Return,
            FlightNumber = command.FlightNumber,
            Carrier = command.Carrier,
            Origin = command.Origin,
            Destination = command.Destination,
            DepartureDate = command.DepartureDate,
            PassengerCount = command.PassengerCount,
            ConfirmationCode = GenerateConfirmationCode(),
            Price = CalculatePrice(command),
            Status = ReservationStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            ConfirmedAt = DateTime.UtcNow
        };

        _dbContext.FlightReservations.Add(reservation);
        await _dbContext.SaveChangesAsync();

        await context.Publish(new ReturnFlightReserved(
            command.CorrelationId,
            command.TripId,
            reservation.Id,
            reservation.FlightNumber,
            reservation.ConfirmationCode!,
            reservation.DepartureDate,
            reservation.Price));
    }

    private static string GenerateConfirmationCode() => $"FL-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    
    private static decimal CalculatePrice(ReserveReturnFlightCommand command) => 250.00m * command.PassengerCount;
}
