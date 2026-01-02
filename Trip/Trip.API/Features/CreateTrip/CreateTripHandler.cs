using MassTransit;
using MediatR;
using Trip.Application.Abstractions;
using Trip.Contracts.DTOs;
using Trip.Contracts.Events;
using Trip.Domain.Entities;

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
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            Status = TripStatus.Pending,
            TotalAmount = request.Details.Payment.Amount,
            CreatedAt = DateTime.UtcNow
        };

        await _tripRepository.AddAsync(tripBooking, cancellationToken);

        // Publish event to start SAGA orchestration
        await _publishEndpoint.Publish(new TripBookingCreated(
            TripId: tripBooking.Id,
            CustomerId: tripBooking.CustomerId,
            CustomerName: tripBooking.CustomerName,
            CustomerEmail: tripBooking.CustomerEmail,
            Origin: request.Details.OutboundFlight.Origin,
            Destination: request.Details.OutboundFlight.Destination,
            DepartureDate: request.Details.OutboundFlight.DepartureDate,
            ReturnDate: request.Details.ReturnFlight.DepartureDate,
            OutboundFlightNumber: request.Details.OutboundFlight.FlightNumber,
            OutboundCarrier: request.Details.OutboundFlight.Carrier,
            ReturnFlightNumber: request.Details.ReturnFlight.FlightNumber,
            ReturnCarrier: request.Details.ReturnFlight.Carrier,
            HotelId: request.Details.Hotel.HotelId,
            HotelName: request.Details.Hotel.HotelName,
            CheckIn: request.Details.Hotel.CheckIn,
            CheckOut: request.Details.Hotel.CheckOut,
            NumberOfGuests: request.Details.Hotel.NumberOfGuests,
            IncludeGroundTransport: request.Details.GroundTransport is not null,
            GroundTransportType: request.Details.GroundTransport?.Type,
            GroundTransportPickupLocation: request.Details.GroundTransport?.PickupLocation,
            GroundTransportDropoffLocation: request.Details.GroundTransport?.DropoffLocation,
            IncludeInsurance: request.Details.IncludeInsurance,
            PaymentMethodId: request.Details.PaymentMethodId,
            TotalAmount: request.Details.Payment.Amount,
            Currency: request.Details.Payment.Currency,
            CreatedAt: tripBooking.CreatedAt), cancellationToken);

        return new TripBookingResponse(
            tripBooking.Id,
            tripBooking.Status.ToString(),
            tripBooking.CreatedAt,
            tripBooking.CompletedAt);
    }
}
