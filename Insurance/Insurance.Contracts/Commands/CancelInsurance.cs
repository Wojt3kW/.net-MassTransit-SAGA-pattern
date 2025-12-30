namespace Insurance.Contracts.Commands;

public record CancelInsurance(
    Guid CorrelationId,
    Guid TripId,
    Guid InsurancePolicyId,
    string Reason);
