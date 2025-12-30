namespace GroundTransport.Contracts.Commands;

public record CancelGroundTransport(
    Guid CorrelationId,
    Guid TripId,
    Guid TransportReservationId,
    string Reason);
