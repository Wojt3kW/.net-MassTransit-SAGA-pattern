namespace Notification.Contracts.DTOs;

/// <summary>
/// Response DTO for paginated list of notification records.
/// </summary>
public record NotificationRecordListResponse(
    IReadOnlyList<NotificationRecordResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
