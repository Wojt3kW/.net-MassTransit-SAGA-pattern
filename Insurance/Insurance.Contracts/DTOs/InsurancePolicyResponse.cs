namespace Insurance.Contracts.DTOs;

/// <summary>
/// Response DTO for insurance policy details.
/// </summary>
public record InsurancePolicyResponse(
    Guid Id,
    Guid TripId,
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    string? PolicyNumber,
    DateTime CoverageStartDate,
    DateTime CoverageEndDate,
    Guid OutboundFlightReservationId,
    Guid ReturnFlightReservationId,
    Guid HotelReservationId,
    decimal TripTotalValue,
    decimal Premium,
    string Status,
    string? CancellationReason,
    DateTime CreatedAt,
    DateTime? IssuedAt,
    DateTime? CancelledAt);
