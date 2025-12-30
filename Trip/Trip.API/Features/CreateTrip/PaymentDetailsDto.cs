namespace Trip.API.Features.CreateTrip;

// ⚠️ WARNING: FOR EDUCATIONAL PURPOSES ONLY!
// In production, card data should be tokenized immediately upon receipt.
// Never store or log full card numbers. Use payment gateway SDKs (Stripe.js, Adyen Web)
// to collect card data directly, bypassing your backend entirely.
public record PaymentDetailsDto(
    string CardNumber,
    string CardHolderName,
    string ExpiryDate,
    string Cvv,
    decimal Amount,
    string Currency);
