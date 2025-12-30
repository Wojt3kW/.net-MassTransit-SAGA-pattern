namespace FlightBooking.Contracts.DTOs;

/// <summary>
/// Response DTO for paginated list of flight reservations.
/// </summary>
public record FlightReservationListResponse(
    IReadOnlyList<FlightReservationResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
