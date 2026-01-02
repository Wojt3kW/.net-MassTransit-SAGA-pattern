namespace Insurance.Contracts.Events;

/// <summary>Published when issuing travel insurance has failed (e.g., underwriting rejection).</summary>
public record InsuranceIssueFailed(
    Guid CorrelationId,
    Guid TripId,
    string Reason);
