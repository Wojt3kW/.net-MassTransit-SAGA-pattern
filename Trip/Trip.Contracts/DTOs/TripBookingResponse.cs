namespace Trip.Contracts.DTOs;

public record TripBookingResponse(
    Guid TripId,
    string Status,
    DateTime CreatedAt,
    DateTime? CompletedAt);
