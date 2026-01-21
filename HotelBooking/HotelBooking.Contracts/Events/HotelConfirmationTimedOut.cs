namespace HotelBooking.Contracts.Events
{
    /// <summary>Published when hotel confirmation has timed out.</summary>
    public record HotelConfirmationTimedOut(
        Guid CorrelationId,
        Guid TripId,
        Guid? HotelReservationId);
}
