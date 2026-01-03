using FlightBooking.Contracts.Events;
using MassTransit;
using Trip.Application.Abstractions;

namespace Trip.API.Consumers;

/// <summary>
/// Handles OutboundFlightReserved events to update trip booking with flight confirmation.
/// </summary>
public class OutboundFlightReservedConsumer : IConsumer<OutboundFlightReserved>
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<OutboundFlightReservedConsumer> _logger;

    public OutboundFlightReservedConsumer(
        ITripRepository tripRepository,
        ILogger<OutboundFlightReservedConsumer> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OutboundFlightReserved> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received OutboundFlightReserved for TripId: {TripId}, ConfirmationCode: {ConfirmationCode}",
            message.TripId,
            message.ConfirmationCode);

        var updated = await _tripRepository.UpdateOutboundFlightConfirmationAsync(
            message.TripId,
            message.ConfirmationCode,
            context.CancellationToken);

        if (!updated)
        {
            _logger.LogWarning(
                "Trip not found: {TripId}. Will retry.",
                message.TripId);
            throw new InvalidOperationException($"Trip {message.TripId} not found. Retrying...");
        }

        _logger.LogInformation(
            "Updated TripId: {TripId} with OutboundFlightConfirmation: {ConfirmationCode}",
            message.TripId,
            message.ConfirmationCode);
    }
}
