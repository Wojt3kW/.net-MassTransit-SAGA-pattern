using MassTransit;
using Notification.Application.Abstractions;
using Notification.Domain.Entities;
using Notification.Contracts.Commands;
using Notification.Contracts.Events;

namespace Notification.API.Features.SendCancellation;

/// <summary>
/// Consumer that handles cancellation notification commands.
/// </summary>
public class SendCancellationConsumer : IConsumer<SendCancellationNotification>
{
    private readonly INotificationRepository _repository;
    private readonly ILogger<SendCancellationConsumer> _logger;

    public SendCancellationConsumer(INotificationRepository repository, ILogger<SendCancellationConsumer> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendCancellationNotification> context)
    {
        var command = context.Message;

        var notification = new NotificationRecord
        {
            Id = Guid.NewGuid(),
            TripId = command.TripId,
            CustomerId = command.CustomerId,
            Type = NotificationType.Cancellation,
            Channel = NotificationChannel.Email,
            Recipient = command.CustomerEmail,
            Subject = "Your Travel Booking Has Been Cancelled",
            Body = $"""
                Dear {command.CustomerName},
                
                Your travel booking has been cancelled as requested.
                
                Reason: {command.CancellationReason}
                
                Any captured payments will be refunded within 5-7 business days.
                
                If you did not request this cancellation, please contact our support team immediately.
                """,
            Status = NotificationStatus.Sent,
            CreatedAt = DateTime.UtcNow,
            SentAt = DateTime.UtcNow
        };

        await _repository.AddAsync(notification, context.CancellationToken);

        _logger.LogInformation("Cancellation notification sent to {Email} for trip {TripId}", 
            command.CustomerEmail, command.TripId);

        await context.Publish(new NotificationSent(
            notification.Id,
            command.TripId,
            "Cancellation",
            "Email",
            DateTime.UtcNow));
    }
}
