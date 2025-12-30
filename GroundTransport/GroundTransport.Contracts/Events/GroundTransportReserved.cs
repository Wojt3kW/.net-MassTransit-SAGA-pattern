namespace GroundTransport.Contracts.Events;

public record GroundTransportReserved(
    Guid CorrelationId,
    Guid TripId,
    Guid TransportReservationId,
    string Type,
    string ConfirmationCode,
    DateTime PickupDate,
    decimal Price);
