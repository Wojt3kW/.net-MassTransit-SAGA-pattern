namespace Payment.Domain.Entities;

public enum PaymentStatus
{
    Pending,
    Authorised,
    Captured,
    Released,
    Refunded,
    Failed
}
