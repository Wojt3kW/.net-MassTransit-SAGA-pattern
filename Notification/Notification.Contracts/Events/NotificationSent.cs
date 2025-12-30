namespace Notification.Contracts.Events;

public record NotificationSent(
    Guid NotificationId,
    Guid TripId,
    string NotificationType,
    string Channel,
    DateTime SentAt);
