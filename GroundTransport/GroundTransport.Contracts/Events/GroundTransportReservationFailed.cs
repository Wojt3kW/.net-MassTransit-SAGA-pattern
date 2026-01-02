namespace GroundTransport.Contracts.Events;

/// <summary>Published when a ground transport reservation has failed (e.g., no vehicles available).</summary>
public record GroundTransportReservationFailed(
    Guid CorrelationId,
    Guid TripId,
    string Reason);
