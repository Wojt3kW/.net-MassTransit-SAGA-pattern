using MassTransit;
using Notification.Application.Abstractions;
using Notification.Domain.Entities;
using Notification.Contracts.Events;
using SendBookingConfirmationCommand = Notification.Contracts.Commands.SendBookingConfirmation;

namespace Notification.API.Features.SendBookingConfirmation;

/// <summary>
/// Consumer that handles booking confirmation notification commands.
/// </summary>
public class SendBookingConfirmationConsumer : IConsumer<SendBookingConfirmationCommand>
{
    private readonly INotificationRepository _repository;
    private readonly ILogger<SendBookingConfirmationConsumer> _logger;

    public SendBookingConfirmationConsumer(INotificationRepository repository, ILogger<SendBookingConfirmationConsumer> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendBookingConfirmationCommand> context)
    {
        var command = context.Message;

        var notification = new NotificationRecord
        {
            Id = Guid.NewGuid(),
            TripId = command.TripId,
            CustomerId = command.CustomerId,
            Type = NotificationType.BookingConfirmation,
            Channel = NotificationChannel.Email,
            Recipient = command.CustomerEmail,
            Subject = "Your Travel Booking Confirmation",
            Body = BuildConfirmationBody(command),
            Status = NotificationStatus.Sent,
            CreatedAt = DateTime.UtcNow,
            SentAt = DateTime.UtcNow
        };

        await _repository.AddAsync(notification, context.CancellationToken);

        _logger.LogInformation("Booking confirmation sent to {Email} for trip {TripId}", 
            command.CustomerEmail, command.TripId);

        await context.Publish(new NotificationSent(
            notification.Id,
            command.TripId,
            "BookingConfirmation",
            "Email",
            DateTime.UtcNow));
    }

    private static string BuildConfirmationBody(SendBookingConfirmationCommand command)
    {
        return $"""
            Dear {command.CustomerName},
            
            Your travel booking has been confirmed!
            
            Booking Details:
            - Outbound Flight: {command.BookingDetails.OutboundFlightConfirmation}
            - Return Flight: {command.BookingDetails.ReturnFlightConfirmation}
            - Hotel: {command.BookingDetails.HotelConfirmation}
            - Ground Transport: {command.BookingDetails.GroundTransportConfirmation ?? "Not included"}
            - Insurance: {command.BookingDetails.InsurancePolicyNumber ?? "Not included"}
            
            Total Amount: {command.BookingDetails.TotalAmount:C}
            
            Thank you for booking with us!
            """;
    }
}
