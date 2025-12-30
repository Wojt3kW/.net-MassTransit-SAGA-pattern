namespace Notification.Contracts.Commands;

public record SendCancellationNotification(
    Guid TripId,
    Guid CustomerId,
    string CustomerEmail,
    string CustomerName,
    string CancellationReason);
