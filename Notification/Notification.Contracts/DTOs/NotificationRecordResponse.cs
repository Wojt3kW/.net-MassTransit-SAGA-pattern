namespace Notification.Contracts.DTOs;

/// <summary>
/// Response DTO for notification record details.
/// </summary>
public record NotificationRecordResponse(
    Guid Id,
    Guid TripId,
    Guid CustomerId,
    string Type,
    string Channel,
    string Recipient,
    string Subject,
    string Body,
    string Status,
    string? ErrorMessage,
    DateTime CreatedAt,
    DateTime? SentAt);
