using FluentValidation;

namespace Trip.API.Features.CreateTrip;

public class CreateTripCommandValidator : AbstractValidator<CreateTripCommand>
{
    public CreateTripCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.CustomerEmail)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Valid customer email is required");

        RuleFor(x => x.Details)
            .NotNull()
            .WithMessage("Trip details are required");

        RuleFor(x => x.Details.OutboundFlight)
            .NotNull()
            .WithMessage("Outbound flight details are required");

        RuleFor(x => x.Details.ReturnFlight)
            .NotNull()
            .WithMessage("Return flight details are required");

        RuleFor(x => x.Details.Hotel)
            .NotNull()
            .WithMessage("Hotel details are required");

        RuleFor(x => x.Details.Payment)
            .NotNull()
            .WithMessage("Payment details are required");

        When(x => x.Details?.Payment is not null, () =>
        {
            RuleFor(x => x.Details.Payment.CardNumber)
                .NotEmpty()
                .CreditCard()
                .WithMessage("Valid card number is required");

            RuleFor(x => x.Details.Payment.CardHolderName)
                .NotEmpty()
                .WithMessage("Card holder name is required");

            RuleFor(x => x.Details.Payment.ExpiryDate)
                .NotEmpty()
                .Matches(@"^\d{2}/\d{2}$")
                .WithMessage("Expiry date must be in MM/YY format");

            RuleFor(x => x.Details.Payment.Cvv)
                .NotEmpty()
                .Matches(@"^\d{3,4}$")
                .WithMessage("CVV must be 3 or 4 digits");

            RuleFor(x => x.Details.Payment.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero");
        });
    }
}
