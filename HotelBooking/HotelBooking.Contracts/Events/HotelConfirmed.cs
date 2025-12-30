namespace HotelBooking.Contracts.Events;

public record HotelConfirmed(
    Guid CorrelationId,
    Guid TripId,
    Guid HotelReservationId,
    DateTime ConfirmedAt);
