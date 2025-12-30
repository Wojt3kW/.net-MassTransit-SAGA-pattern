namespace Notification.Domain.Entities;

/// <summary>
/// Represents a notification sent to the customer.
/// </summary>
public class NotificationRecord
{
    /// <summary>Unique identifier of the notification record.</summary>
    public Guid Id { get; set; }
    
    /// <summary>Reference to the related trip booking.</summary>
    public Guid TripId { get; set; }
    
    /// <summary>Customer who receives the notification.</summary>
    public Guid CustomerId { get; set; }
    
    /// <summary>Type of notification (e.g., Confirmation, Failure).</summary>
    public NotificationType Type { get; set; }
    
    /// <summary>Communication channel (Email, SMS, Push).</summary>
    public NotificationChannel Channel { get; set; }
    
    /// <summary>Recipient address (email, phone number, or device ID).</summary>
    public string Recipient { get; set; } = default!;
    
    /// <summary>Notification subject or title.</summary>
    public string Subject { get; set; } = default!;
    
    /// <summary>Full notification message body.</summary>
    public string Body { get; set; } = default!;
    
    /// <summary>Current delivery status of the notification.</summary>
    public NotificationStatus Status { get; set; }
    
    /// <summary>Error message if delivery failed.</summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>Timestamp when the notification was created.</summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>Timestamp when the notification was sent.</summary>
    public DateTime? SentAt { get; set; }
}
