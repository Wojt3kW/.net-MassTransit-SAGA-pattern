using Notification.Application.Abstractions;
using Notification.Contracts.DTOs;

namespace Notification.API.Features.GetNotification;

/// <summary>
/// Endpoint to get a single notification record by ID.
/// </summary>
public class GetNotificationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/notifications/{id:guid}", async (
            Guid id,
            INotificationRepository repository,
            CancellationToken cancellationToken) =>
        {
            var notification = await repository.GetByIdAsync(id, cancellationToken);

            if (notification is null)
                return Results.NotFound();

            var response = new NotificationRecordResponse(
                notification.Id,
                notification.TripId,
                notification.CustomerId,
                notification.Type.ToString(),
                notification.Channel.ToString(),
                notification.Recipient,
                notification.Subject,
                notification.Body,
                notification.Status.ToString(),
                notification.ErrorMessage,
                notification.CreatedAt,
                notification.SentAt);

            return Results.Ok(response);
        })
        .WithName("GetNotification")
        .WithTags("Notifications")
        .Produces<NotificationRecordResponse>()
        .Produces(StatusCodes.Status404NotFound);
    }
}
