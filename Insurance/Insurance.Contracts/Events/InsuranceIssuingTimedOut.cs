namespace Insurance.Contracts.Events
{
    /// <summary>Published when an insurance issuing has timed out.</summary>
    public record InsuranceIssuingTimedOut(
        Guid CorrelationId,
        Guid TripId);
}
