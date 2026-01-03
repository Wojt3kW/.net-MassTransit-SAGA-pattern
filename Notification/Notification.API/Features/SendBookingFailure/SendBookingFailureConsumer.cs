using MassTransit;
using Notification.Application.Abstractions;
using Notification.Domain.Entities;
using Notification.Contracts.Commands;
using Notification.Contracts.Events;

namespace Notification.API.Features.SendBookingFailure;

/// <summary>
/// Consumer that handles booking failure notification commands.
/// </summary>
public class SendBookingFailureConsumer : IConsumer<SendBookingFailureNotification>
{
    private readonly INotificationRepository _repository;
    private readonly ILogger<SendBookingFailureConsumer> _logger;

    public SendBookingFailureConsumer(INotificationRepository repository, ILogger<SendBookingFailureConsumer> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendBookingFailureNotification> context)
    {
        var command = context.Message;

        var notification = new NotificationRecord
        {
            Id = Guid.NewGuid(),
            TripId = command.TripId,
            CustomerId = command.CustomerId,
            Type = NotificationType.BookingFailure,
            Channel = NotificationChannel.Email,
            Recipient = command.CustomerEmail,
            Subject = "Your Travel Booking Could Not Be Completed",
            Body = $"""
                Dear {command.CustomerName},
                
                Unfortunately, we were unable to complete your travel booking.
                
                Reason: {command.FailureReason}
                
                Any authorised payments have been released back to your card.
                
                Please try again or contact our support team for assistance.
                """,
            Status = NotificationStatus.Sent,
            CreatedAt = DateTime.UtcNow,
            SentAt = DateTime.UtcNow
        };

        await _repository.AddAsync(notification, context.CancellationToken);

        _logger.LogInformation("Booking failure notification sent to {Email} for trip {TripId}", 
            command.CustomerEmail, command.TripId);

        await context.Publish(new NotificationSent(
            notification.Id,
            command.TripId,
            "BookingFailure",
            "Email",
            DateTime.UtcNow));
    }
}
