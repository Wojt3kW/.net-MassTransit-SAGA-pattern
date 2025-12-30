using MassTransit;
using MediatR;
using Trip.Application.Abstractions;
using Trip.Domain.Entities;
using Trip.Contracts.Commands;
using Trip.Contracts.DTOs;

namespace Trip.API.Features.CreateTrip;

public class CreateTripHandler : IRequestHandler<CreateTripCommand, TripBookingResponse>
{
    private readonly ITripRepository _tripRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateTripHandler(ITripRepository tripRepository, IPublishEndpoint publishEndpoint)
    {
        _tripRepository = tripRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<TripBookingResponse> Handle(CreateTripCommand request, CancellationToken cancellationToken)
    {
        var tripBooking = new TripBooking
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            CustomerEmail = request.CustomerEmail,
            Status = TripStatus.Pending,
            TotalAmount = request.Details.Payment.Amount,
            CreatedAt = DateTime.UtcNow
        };

        await _tripRepository.AddAsync(tripBooking, cancellationToken);

        // Publish event to start SAGA
        await _publishEndpoint.Publish(new CreateTripBooking(
            tripBooking.Id,
            tripBooking.CustomerId,
            tripBooking.CustomerEmail,
            new Contracts.Commands.TripDetails(
                new Contracts.Commands.FlightDetails(
                    request.Details.OutboundFlight.Origin,
                    request.Details.OutboundFlight.Destination,
                    request.Details.OutboundFlight.DepartureDate,
                    request.Details.OutboundFlight.FlightNumber,
                    request.Details.OutboundFlight.Carrier),
                new Contracts.Commands.FlightDetails(
                    request.Details.ReturnFlight.Origin,
                    request.Details.ReturnFlight.Destination,
                    request.Details.ReturnFlight.DepartureDate,
                    request.Details.ReturnFlight.FlightNumber,
                    request.Details.ReturnFlight.Carrier),
                new Contracts.Commands.HotelDetails(
                    request.Details.Hotel.HotelId,
                    request.Details.Hotel.HotelName,
                    request.Details.Hotel.CheckIn,
                    request.Details.Hotel.CheckOut,
                    request.Details.Hotel.NumberOfGuests),
                request.Details.GroundTransport is not null
                    ? new Contracts.Commands.GroundTransportDetails(
                        request.Details.GroundTransport.Type,
                        request.Details.GroundTransport.PickupLocation,
                        request.Details.GroundTransport.DropoffLocation,
                        request.Details.GroundTransport.PickupDate)
                    : null,
                request.Details.IncludeInsurance,
                new Contracts.Commands.PaymentDetails(
                    request.Details.Payment.CardNumber,
                    request.Details.Payment.CardHolderName,
                    request.Details.Payment.ExpiryDate,
                    request.Details.Payment.Cvv,
                    request.Details.Payment.Amount,
                    request.Details.Payment.Currency))),
            cancellationToken);

        return new TripBookingResponse(
            tripBooking.Id,
            tripBooking.Status.ToString(),
            tripBooking.CreatedAt,
            tripBooking.CompletedAt);
    }
}
