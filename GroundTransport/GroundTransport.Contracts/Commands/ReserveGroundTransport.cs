namespace GroundTransport.Contracts.Commands;

public record ReserveGroundTransport(
    Guid CorrelationId,
    Guid TripId,
    string Type,
    string PickupLocation,
    string DropoffLocation,
    DateTime PickupDate,
    int PassengerCount);
