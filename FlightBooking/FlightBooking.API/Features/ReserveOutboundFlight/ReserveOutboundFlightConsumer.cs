using FlightBooking.Domain.Entities;
using FlightBooking.Infrastructure.Persistence;
using FlightBooking.Contracts.Events;
using MassTransit;
using ReserveOutboundFlightCommand = FlightBooking.Contracts.Commands.ReserveOutboundFlight;

namespace FlightBooking.API.Features.ReserveOutboundFlight;

public class ReserveOutboundFlightConsumer : IConsumer<ReserveOutboundFlightCommand>
{
    private readonly FlightBookingDbContext _dbContext;

    public ReserveOutboundFlightConsumer(FlightBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ReserveOutboundFlightCommand> context)
    {
        var command = context.Message;

        // Simulate flight reservation (in real scenario, call external airline API)
        var reservation = new FlightReservation
        {
            Id = Guid.NewGuid(),
            TripId = command.TripId,
            Type = FlightType.Outbound,
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

        await context.Publish(new OutboundFlightReserved(
            command.CorrelationId,
            command.TripId,
            reservation.Id,
            reservation.FlightNumber,
            reservation.ConfirmationCode!,
            reservation.DepartureDate,
            reservation.Price));
    }

    private static string GenerateConfirmationCode() => $"FL-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    
    private static decimal CalculatePrice(ReserveOutboundFlightCommand command) => 250.00m * command.PassengerCount;
}
