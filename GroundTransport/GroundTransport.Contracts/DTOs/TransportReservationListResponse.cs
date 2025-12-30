namespace GroundTransport.Contracts.DTOs;

/// <summary>
/// Response DTO for paginated list of transport reservations.
/// </summary>
public record TransportReservationListResponse(
    IReadOnlyList<TransportReservationResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
