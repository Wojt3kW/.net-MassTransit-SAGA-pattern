namespace Insurance.Contracts.Events;

public record InsuranceCancelled(
    Guid CorrelationId,
    Guid TripId,
    Guid InsurancePolicyId,
    DateTime CancelledAt);
