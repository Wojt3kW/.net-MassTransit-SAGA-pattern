namespace HotelBooking.Contracts.Events
{
    /// <summary>Published when hotel reservation has timed out.</summary>
    public record HotelReservationTimedOut(
        Guid CorrelationId,
        Guid TripId);
}
