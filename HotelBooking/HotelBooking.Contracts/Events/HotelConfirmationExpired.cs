namespace HotelBooking.Contracts.Events;

public record HotelConfirmationExpired(
    Guid CorrelationId,
    Guid TripId,
    Guid HotelReservationId);
