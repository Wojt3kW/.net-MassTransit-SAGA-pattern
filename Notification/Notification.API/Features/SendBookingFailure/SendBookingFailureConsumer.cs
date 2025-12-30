using MassTransit;
using Notification.Domain.Entities;
using Notification.Infrastructure.Persistence;
using Notification.Contracts.Commands;
using Notification.Contracts.Events;

namespace Notification.API.Features.SendBookingFailure;

public class SendBookingFailureConsumer : IConsumer<SendBookingFailureNotification>
{
    private readonly NotificationDbContext _dbContext;
    private readonly ILogger<SendBookingFailureConsumer> _logger;

    public SendBookingFailureConsumer(NotificationDbContext dbContext, ILogger<SendBookingFailureConsumer> logger)
    {
        _dbContext = dbContext;
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

        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync();

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
