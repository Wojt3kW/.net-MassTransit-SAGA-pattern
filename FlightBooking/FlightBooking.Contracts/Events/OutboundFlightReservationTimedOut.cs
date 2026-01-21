namespace FlightBooking.Contracts.Events
{
    /// <summary>Published when outbound flight reservation has timed out.</summary>
    public record OutboundFlightReservationTimedOut(
        Guid CorrelationId,
        Guid TripId);
}
