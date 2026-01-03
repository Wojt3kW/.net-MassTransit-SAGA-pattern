namespace Insurance.Contracts.Events;

/// <summary>Published when a travel insurance policy has been cancelled during compensation.</summary>
public record InsuranceCancelled(
    Guid CorrelationId,
    Guid TripId,
    Guid InsurancePolicyId,
    DateTime CancelledAt,
    string Reason);
