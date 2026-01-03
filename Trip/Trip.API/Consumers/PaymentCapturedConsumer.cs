using MassTransit;
using Payment.Contracts.Events;
using Trip.Application.Abstractions;

namespace Trip.API.Consumers;

/// <summary>
/// Handles PaymentCaptured events to update trip booking with payment confirmation.
/// </summary>
public class PaymentCapturedConsumer : IConsumer<PaymentCaptured>
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<PaymentCapturedConsumer> _logger;

    public PaymentCapturedConsumer(
        ITripRepository tripRepository,
        ILogger<PaymentCapturedConsumer> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentCaptured> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received PaymentCaptured for TripId: {TripId}, PaymentId: {PaymentId}",
            message.TripId,
            message.PaymentId);

        var updated = await _tripRepository.UpdatePaymentConfirmationAsync(
            message.TripId,
            message.PaymentId.ToString(),
            context.CancellationToken);

        if (!updated)
        {
            _logger.LogWarning(
                "Trip not found: {TripId}. Will retry.",
                message.TripId);
            throw new InvalidOperationException($"Trip {message.TripId} not found. Retrying...");
        }

        _logger.LogInformation(
            "Updated TripId: {TripId} with PaymentConfirmation: {PaymentId}",
            message.TripId,
            message.PaymentId);
    }
}
