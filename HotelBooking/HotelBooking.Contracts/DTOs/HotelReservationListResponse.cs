namespace HotelBooking.Contracts.DTOs;

/// <summary>
/// Response DTO for paginated list of hotel reservations.
/// </summary>
public record HotelReservationListResponse(
    IReadOnlyList<HotelReservationResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
