namespace GroundTransport.Contracts.Events;

public record GroundTransportCancelled(
    Guid CorrelationId,
    Guid TripId,
    Guid TransportReservationId,
    DateTime CancelledAt);
