namespace GroundTransport.Contracts.Events;

/// <summary>Published when ground transport (airport transfer or car rental) has been reserved.</summary>
public record GroundTransportReserved(
    Guid CorrelationId,
    Guid TripId,
    Guid TransportReservationId,
    string Type,
    string ConfirmationCode,
    DateTime PickupDate,
    decimal Price);
