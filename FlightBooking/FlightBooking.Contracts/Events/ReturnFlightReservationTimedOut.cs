namespace FlightBooking.Contracts.Events
{
    /// <summary>Published when return flight reservation has timed out.</summary>
    public record ReturnFlightReservationTimedOut(
        Guid CorrelationId,
        Guid TripId);
}
