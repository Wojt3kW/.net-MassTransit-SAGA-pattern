namespace HotelBooking.Contracts.DTOs;

/// <summary>
/// Response DTO for hotel reservation details.
/// </summary>
public record HotelReservationResponse(
    Guid Id,
    Guid TripId,
    string HotelId,
    string HotelName,
    DateTime CheckIn,
    DateTime CheckOut,
    int NumberOfGuests,
    string GuestName,
    string GuestEmail,
    string? ConfirmationCode,
    decimal PricePerNight,
    decimal TotalPrice,
    string Status,
    string? CancellationReason,
    DateTime CreatedAt,
    DateTime? ConfirmedAt,
    DateTime? CancelledAt,
    DateTime? ExpiresAt);
