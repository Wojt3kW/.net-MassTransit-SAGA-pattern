using Notification.Domain.Entities;

namespace Notification.Application.Abstractions;

/// <summary>
/// Repository interface for notification record persistence operations.
/// </summary>
public interface INotificationRepository
{
    /// <summary>Gets a notification record by its unique identifier.</summary>
    Task<NotificationRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>Gets all notification records with optional filtering and pagination.</summary>
    Task<(IReadOnlyList<NotificationRecord> Items, int TotalCount)> GetAllAsync(
        Guid? tripId = null,
        NotificationStatus? status = null,
        NotificationType? type = null,
        NotificationChannel? channel = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    
    /// <summary>Adds a new notification record to the database.</summary>
    Task<NotificationRecord> AddAsync(NotificationRecord notification, CancellationToken cancellationToken = default);
    
    /// <summary>Updates an existing notification record.</summary>
    Task UpdateAsync(NotificationRecord notification, CancellationToken cancellationToken = default);
    
    /// <summary>Deletes a notification record by its unique identifier.</summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>Gets all notification records for a specific trip.</summary>
    Task<IReadOnlyList<NotificationRecord>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default);
    
    /// <summary>Gets all pending notification records awaiting delivery.</summary>
    Task<IReadOnlyList<NotificationRecord>> GetPendingNotificationsAsync(CancellationToken cancellationToken = default);
}
