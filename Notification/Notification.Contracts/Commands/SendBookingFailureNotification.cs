namespace Notification.Contracts.Commands;

public record SendBookingFailureNotification(
    Guid TripId,
    Guid CustomerId,
    string CustomerEmail,
    string CustomerName,
    string FailureReason);
