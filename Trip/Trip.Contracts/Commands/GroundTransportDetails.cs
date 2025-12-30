namespace Trip.Contracts.Commands;

public record GroundTransportDetails(
    string Type,
    string PickupLocation,
    string DropoffLocation,
    DateTime PickupDate);
