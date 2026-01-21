namespace Payment.Contracts.Events
{
    /// <summary>Published when payment authorisation has timed out.</summary>
    public record PaymentAuthorisationTimedOut(
        Guid CorrelationId,
        Guid TripId);
}
