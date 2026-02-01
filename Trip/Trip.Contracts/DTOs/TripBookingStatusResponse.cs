namespace Trip.Contracts.DTOs;

public record TripBookingStatusResponse(
    Guid TripId,
    Guid? CustomerId,
    string? CustomerEmail,
    string? CustomerName,
    string Status,
    string? OutboundFlightStatus,
    string? ReturnFlightStatus,
    string? HotelStatus,
    string? GroundTransportStatus,
    string? InsuranceStatus,
    string? PaymentStatus,
    decimal? TotalAmount,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    string? FailureReason);
