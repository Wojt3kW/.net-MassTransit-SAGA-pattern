using FlightBooking.Contracts.Events;
using MassTransit;
using Trip.Application.Abstractions;

namespace Trip.API.Consumers;

/// <summary>
/// Handles ReturnFlightReserved events to update trip booking with flight confirmation.
/// </summary>
public class ReturnFlightReservedConsumer : IConsumer<ReturnFlightReserved>
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<ReturnFlightReservedConsumer> _logger;

    public ReturnFlightReservedConsumer(
        ITripRepository tripRepository,
        ILogger<ReturnFlightReservedConsumer> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReturnFlightReserved> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received ReturnFlightReserved for TripId: {TripId}, ConfirmationCode: {ConfirmationCode}",
            message.TripId,
            message.ConfirmationCode);

        var updated = await _tripRepository.UpdateReturnFlightConfirmationAsync(
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
            "Updated TripId: {TripId} with ReturnFlightConfirmation: {ConfirmationCode}",
            message.TripId,
            message.ConfirmationCode);
    }
}
