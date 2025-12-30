namespace HotelBooking.Contracts.Events;

public record HotelCancelled(
    Guid CorrelationId,
    Guid TripId,
    Guid HotelReservationId,
    DateTime CancelledAt);
