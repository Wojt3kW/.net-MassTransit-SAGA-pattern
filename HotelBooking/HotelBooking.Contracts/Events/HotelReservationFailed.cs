namespace HotelBooking.Contracts.Events;

public record HotelReservationFailed(
    Guid CorrelationId,
    Guid TripId,
    string Reason);
