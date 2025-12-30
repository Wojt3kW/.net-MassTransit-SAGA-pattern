namespace Payment.Contracts.Commands;

// ⚠️ WARNING: FOR EDUCATIONAL PURPOSES ONLY!
// In production, NEVER transmit full card numbers or CVV through message queues.
// This violates PCI-DSS compliance. Use payment tokenization instead:
// 1. API receives card data and immediately tokenizes via payment gateway
// 2. Only the payment token is published to the message queue
// 3. Payment service uses the token to capture/refund without seeing card data
public record AuthorisePayment(
    Guid CorrelationId,
    Guid TripId,
    Guid CustomerId,
    string CardNumber,
    string CardHolderName,
    string ExpiryDate,
    string Cvv,
    decimal Amount,
    string Currency);
