namespace Insurance.Contracts.Events;

public record InsuranceIssued(
    Guid CorrelationId,
    Guid TripId,
    Guid InsurancePolicyId,
    string PolicyNumber,
    DateTime CoverageStartDate,
    DateTime CoverageEndDate,
    decimal Premium);
