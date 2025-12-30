using Notification.Application.Abstractions;

namespace Notification.API.Features.DeleteNotification;

/// <summary>
/// Endpoint to delete a notification record.
/// </summary>
public class DeleteNotificationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/notifications/{id:guid}", async (
            Guid id,
            INotificationRepository repository,
            CancellationToken cancellationToken) =>
        {
            var deleted = await repository.DeleteAsync(id, cancellationToken);

            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteNotification")
        .WithTags("Notifications")
        .WithOpenApi()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
