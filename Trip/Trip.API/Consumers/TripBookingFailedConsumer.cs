using MassTransit;
using Trip.Application.Abstractions;
using Trip.Contracts.Events;
using Trip.Domain.Entities;

namespace Trip.API.Consumers;

/// <summary>
/// Handles TripBookingFailed events to update trip status to Failed.
/// </summary>
public class TripBookingFailedConsumer : IConsumer<TripBookingFailed>
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<TripBookingFailedConsumer> _logger;

    public TripBookingFailedConsumer(
        ITripRepository tripRepository,
        ILogger<TripBookingFailedConsumer> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TripBookingFailed> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received TripBookingFailed for TripId: {TripId}, Reason: {Reason}, FailedAt: {FailedAt}",
            message.TripId,
            message.Reason,
            message.FailedAt);

        var updated = await _tripRepository.UpdateStatusAsync(
            message.TripId,
            TripStatus.Failed,
            message.FailedAt,
            context.CancellationToken);

        if (!updated)
        {
            _logger.LogWarning(
                "Trip not found: {TripId}. Will retry.",
                message.TripId);
            throw new InvalidOperationException($"Trip {message.TripId} not found. Retrying...");
        }

        _logger.LogInformation(
            "Trip {TripId} marked as Failed at {FailedAt}. Reason: {Reason}",
            message.TripId,
            message.FailedAt,
            message.Reason);
    }
}
