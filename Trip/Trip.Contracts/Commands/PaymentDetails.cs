namespace Trip.Contracts.Commands;

// ⚠️ WARNING: FOR EDUCATIONAL PURPOSES ONLY!
// In production, NEVER transmit full card numbers or CVV through message queues.
// This violates PCI-DSS compliance. Use payment tokenization instead:
// 1. Send card data directly to payment gateway (Stripe, Adyen, etc.)
// 2. Receive a token in response
// 3. Use only the token in message contracts
public record PaymentDetails(
    string CardNumber,
    string CardHolderName,
    string ExpiryDate,
    string Cvv,
    decimal Amount,
    string Currency);
