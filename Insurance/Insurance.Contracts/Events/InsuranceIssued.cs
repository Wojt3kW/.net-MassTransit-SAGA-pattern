namespace Insurance.Contracts.Events;

/// <summary>Published when a travel insurance policy has been successfully issued.</summary>
public record InsuranceIssued(
    Guid CorrelationId,
    Guid TripId,
    Guid InsurancePolicyId,
    string PolicyNumber,
    DateTime CoverageStartDate,
    DateTime CoverageEndDate,
    decimal Premium);
