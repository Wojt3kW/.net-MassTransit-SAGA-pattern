namespace Payment.Contracts.Events
{
    /// <summary>Published when a payment capture has timed out.</summary>
    public record PaymentCaptureTimedOut(
        Guid CorrelationId,
        Guid TripId,
        Guid? PaymentTransactionId);
}
