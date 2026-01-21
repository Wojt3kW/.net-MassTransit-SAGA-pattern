namespace GroundTransport.Contracts.Events
{
    /// <summary>Published when a ground transport reservation has timed out.</summary>
    public record GroundTransportReservationTimedOut(
        Guid CorrelationId,
        Guid TripId);
}
