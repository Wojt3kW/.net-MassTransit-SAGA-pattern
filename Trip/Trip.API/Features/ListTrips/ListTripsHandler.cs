using MediatR;
using Trip.Application.Abstractions;
using Trip.Contracts.DTOs;
using Trip.Domain.Entities;

namespace Trip.API.Features.ListTrips;

/// <summary>
/// Handler for retrieving a paginated list of trips.
/// </summary>
public class ListTripsHandler : IRequestHandler<ListTripsQuery, PagedTripsResponse>
{
    private readonly ITripRepository _tripRepository;

    public ListTripsHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<PagedTripsResponse> Handle(ListTripsQuery request, CancellationToken cancellationToken)
    {
        TripStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<TripStatus>(request.Status, ignoreCase: true, out var parsed))
        {
            statusFilter = parsed;
        }

        var (items, totalCount) = await _tripRepository.GetAllAsync(
            status: statusFilter,
            customerId: request.CustomerId,
            page: request.Page,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        var tripResponses = items.Select(trip => new TripBookingStatusResponse(
            TripId: trip.Id,
            CustomerId: trip.CustomerId,
            CustomerEmail: trip.CustomerEmail,
            CustomerName: trip.CustomerName,
            Status: trip.Status.ToString(),
            OutboundFlightStatus: trip.OutboundFlightConfirmation is not null ? "Confirmed" : "Pending",
            ReturnFlightStatus: trip.ReturnFlightConfirmation is not null ? "Confirmed" : "Pending",
            HotelStatus: trip.HotelConfirmation is not null ? "Confirmed" : "Pending",
            GroundTransportStatus: trip.GroundTransportConfirmation is not null ? "Confirmed" : "NotRequested",
            InsuranceStatus: trip.InsurancePolicyNumber is not null ? "Issued" : "Pending",
            PaymentStatus: trip.PaymentConfirmation is not null ? "Captured" : "Pending",
            CreatedAt: trip.CreatedAt,
            CompletedAt: trip.CompletedAt,
            FailureReason: trip.FailureReason,
            TotalAmount: trip.TotalAmount)).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new PagedTripsResponse(
            Items: tripResponses,
            TotalCount: totalCount,
            Page: request.Page,
            PageSize: request.PageSize,
            TotalPages: totalPages);
    }
}
