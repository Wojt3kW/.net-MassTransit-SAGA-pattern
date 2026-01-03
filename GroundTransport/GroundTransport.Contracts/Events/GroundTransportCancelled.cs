namespace GroundTransport.Contracts.Events;

/// <summary>Published when a ground transport reservation has been cancelled during compensation.</summary>
public record GroundTransportCancelled(
    Guid CorrelationId,
    Guid TripId,
    Guid TransportReservationId,
    DateTime CancelledAt,
    string Reason);
