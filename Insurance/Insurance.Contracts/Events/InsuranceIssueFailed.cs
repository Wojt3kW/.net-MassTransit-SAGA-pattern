namespace Insurance.Contracts.Events;

public record InsuranceIssueFailed(
    Guid CorrelationId,
    Guid TripId,
    string Reason);
