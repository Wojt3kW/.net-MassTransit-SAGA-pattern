using MassTransit;
using Trip.Application.Abstractions;
using Trip.Contracts.Events;
using Trip.Domain.Entities;

namespace Trip.API.Consumers;

/// <summary>
/// Handles TripBookingRefunded events to update trip status to Refunded.
/// </summary>
public class TripBookingRefundedConsumer : IConsumer<TripBookingRefunded>
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<TripBookingRefundedConsumer> _logger;

    public TripBookingRefundedConsumer(
        ITripRepository tripRepository,
        ILogger<TripBookingRefundedConsumer> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TripBookingRefunded> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received TripBookingRefunded for TripId: {TripId}, Amount: {Amount}, RefundedAt: {RefundedAt}",
            message.TripId,
            message.RefundedAmount,
            message.RefundedAt);

        var updated = await _tripRepository.UpdateStatusAsync(
            message.TripId,
            TripStatus.Refunded,
            message.RefundedAt,
            context.CancellationToken);

        if (!updated)
        {
            _logger.LogWarning(
                "Trip not found: {TripId}. Will retry.",
                message.TripId);
            throw new InvalidOperationException($"Trip {message.TripId} not found. Retrying...");
        }

        _logger.LogInformation(
            "Trip {TripId} marked as Refunded (Amount: {Amount}) at {RefundedAt}",
            message.TripId,
            message.RefundedAmount,
            message.RefundedAt);
    }
}
