namespace Notification.Contracts.Events;

/// <summary>Published when a customer notification has been sent (email, SMS, or push).</summary>
public record NotificationSent(
    Guid NotificationId,
    Guid TripId,
    string NotificationType,
    string Channel,
    DateTime SentAt);
