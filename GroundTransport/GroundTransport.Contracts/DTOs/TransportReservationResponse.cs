namespace GroundTransport.Contracts.DTOs;

/// <summary>
/// Response DTO for transport reservation details.
/// </summary>
public record TransportReservationResponse(
    Guid Id,
    Guid TripId,
    string Type,
    string PickupLocation,
    string DropoffLocation,
    DateTime PickupDate,
    int PassengerCount,
    string? ConfirmationCode,
    decimal Price,
    string Status,
    string? CancellationReason,
    DateTime CreatedAt,
    DateTime? ConfirmedAt,
    DateTime? CancelledAt);
