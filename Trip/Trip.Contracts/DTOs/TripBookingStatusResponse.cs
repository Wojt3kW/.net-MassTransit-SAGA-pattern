namespace Trip.Contracts.DTOs;

public record TripBookingStatusResponse(
    Guid TripId,
    string Status,
    string? OutboundFlightStatus,
    string? ReturnFlightStatus,
    string? HotelStatus,
    string? GroundTransportStatus,
    string? InsuranceStatus,
    string? PaymentStatus,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    string? FailureReason);
