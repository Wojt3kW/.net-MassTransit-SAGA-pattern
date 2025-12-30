using Microsoft.EntityFrameworkCore;
using Notification.Application.Abstractions;
using Notification.Domain.Entities;
using Notification.Infrastructure.Persistence;

namespace Notification.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for notification record persistence operations.
/// </summary>
public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _dbContext;

    public NotificationRepository(NotificationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<NotificationRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<NotificationRecord> Items, int TotalCount)> GetAllAsync(
        Guid? tripId = null,
        NotificationStatus? status = null,
        NotificationType? type = null,
        NotificationChannel? channel = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Notifications.AsQueryable();

        if (tripId.HasValue)
            query = query.Where(n => n.TripId == tripId.Value);

        if (status.HasValue)
            query = query.Where(n => n.Status == status.Value);

        if (type.HasValue)
            query = query.Where(n => n.Type == type.Value);

        if (channel.HasValue)
            query = query.Where(n => n.Channel == channel.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<NotificationRecord> AddAsync(NotificationRecord notification, CancellationToken cancellationToken = default)
    {
        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return notification;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(NotificationRecord notification, CancellationToken cancellationToken = default)
    {
        _dbContext.Notifications.Update(notification);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _dbContext.Notifications
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

        if (notification is null)
            return false;

        _dbContext.Notifications.Remove(notification);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<NotificationRecord>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .Where(n => n.TripId == tripId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<NotificationRecord>> GetPendingNotificationsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .Where(n => n.Status == NotificationStatus.Pending)
            .ToListAsync(cancellationToken);
    }
}
