namespace FlightBooking.Contracts.DTOs;

/// <summary>
/// Response DTO for flight reservation details.
/// </summary>
public record FlightReservationResponse(
    Guid Id,
    Guid TripId,
    string Type,
    string FlightNumber,
    string Carrier,
    string Origin,
    string Destination,
    DateTime DepartureDate,
    string? ConfirmationCode,
    decimal Price,
    int PassengerCount,
    string Status,
    string? CancellationReason,
    DateTime CreatedAt,
    DateTime? ConfirmedAt,
    DateTime? CancelledAt);
