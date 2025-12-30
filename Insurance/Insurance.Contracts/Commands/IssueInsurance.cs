namespace Insurance.Contracts.Commands;

public record IssueInsurance(
    Guid CorrelationId,
    Guid TripId,
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    Guid OutboundFlightReservationId,
    Guid ReturnFlightReservationId,
    Guid HotelReservationId,
    DateTime CoverageStartDate,
    DateTime CoverageEndDate,
    decimal TripTotalValue);
