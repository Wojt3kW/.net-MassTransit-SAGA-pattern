namespace GroundTransport.Contracts.Events;

public record GroundTransportReservationFailed(
    Guid CorrelationId,
    Guid TripId,
    string Reason);
