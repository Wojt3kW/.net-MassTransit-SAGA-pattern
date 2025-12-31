using Notification.Application.Abstractions;
using Notification.Contracts.DTOs;
using Notification.Domain.Entities;

namespace Notification.API.Features.GetNotifications;

/// <summary>
/// Endpoint to get all notification records with optional filtering.
/// </summary>
public class GetNotificationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/notifications", async (
            INotificationRepository repository,
            Guid? tripId = null,
            string? status = null,
            string? type = null,
            string? channel = null,
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default) =>
        {
            NotificationStatus? statusEnum = null;
            NotificationType? typeEnum = null;
            NotificationChannel? channelEnum = null;

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<NotificationStatus>(status, true, out var parsedStatus))
                statusEnum = parsedStatus;

            if (!string.IsNullOrEmpty(type) && Enum.TryParse<NotificationType>(type, true, out var parsedType))
                typeEnum = parsedType;

            if (!string.IsNullOrEmpty(channel) && Enum.TryParse<NotificationChannel>(channel, true, out var parsedChannel))
                channelEnum = parsedChannel;

            var (items, totalCount) = await repository.GetAllAsync(
                tripId,
                statusEnum,
                typeEnum,
                channelEnum,
                page,
                pageSize,
                cancellationToken);

            var response = items.Select(n => new NotificationRecordResponse(
                n.Id,
                n.TripId,
                n.CustomerId,
                n.Type.ToString(),
                n.Channel.ToString(),
                n.Recipient,
                n.Subject,
                n.Body,
                n.Status.ToString(),
                n.ErrorMessage,
                n.CreatedAt,
                n.SentAt)).ToList();

            return Results.Ok(new NotificationRecordListResponse(response, totalCount, page, pageSize));
        })
        .WithName("GetNotifications")
        .WithTags("Notifications")
        .Produces<NotificationRecordListResponse>();
    }
}
