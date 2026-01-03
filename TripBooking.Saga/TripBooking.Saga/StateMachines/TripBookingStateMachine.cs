using FlightBooking.Contracts.Commands;
using FlightBooking.Contracts.Events;
using GroundTransport.Contracts.Commands;
using GroundTransport.Contracts.Events;
using HotelBooking.Contracts.Commands;
using HotelBooking.Contracts.Events;
using Insurance.Contracts.Commands;
using Insurance.Contracts.Events;
using MassTransit;
using Notification.Contracts.Events;
using Payment.Contracts.Commands;
using Payment.Contracts.Events;
using Trip.Contracts.Events;
using TripBooking.Saga.States;

namespace TripBooking.Saga.StateMachines;

/// <summary>
/// Orchestrates the trip booking process using the Saga pattern.
/// </summary>
public class TripBookingStateMachine : MassTransitStateMachine<TripBookingSagaState>
{
    public State AwaitingPaymentAuthorisation { get; private set; } = null!;
    public State AwaitingOutboundFlight { get; private set; } = null!;
    public State AwaitingReturnFlight { get; private set; } = null!;
    public State AwaitingHotel { get; private set; } = null!;
    public State AwaitingHotelConfirmation { get; private set; } = null!;
    public State AwaitingGroundTransport { get; private set; } = null!;
    public State AwaitingInsurance { get; private set; } = null!;
    public State AwaitingPaymentCapture { get; private set; } = null!;
    public State Completed { get; private set; } = null!;

    public State Failed { get; private set; } = null!;
    public State Compensating { get; private set; } = null!;
    public State CompensatingInsurance { get; private set; } = null!;
    public State CompensatingGroundTransport { get; private set; } = null!;
    public State CompensatingHotel { get; private set; } = null!;
    public State CompensatingReturnFlight { get; private set; } = null!;
    public State CompensatingOutboundFlight { get; private set; } = null!;
    public State ReleasingPayment { get; private set; } = null!;
    public State Cancelled { get; private set; } = null!;
    public State TimedOut { get; private set; } = null!;
    public State ManualReviewRequired { get; private set; } = null!;

    public Event<TripBookingCreated> TripBookingCreated { get; private set; } = null!;
    public Event<TripBookingCompleted> TripBookingCompleted { get; private set; } = null!;
    public Event<TripBookingFailed> TripBookingFailed { get; private set; } = null!;
    public Event<TripBookingCancelled> TripBookingCancelled { get; private set; } = null!;

    public Event<PaymentAuthorised> PaymentAuthorised { get; private set; } = null!;
    public Event<PaymentAuthorisationFailed> PaymentAuthorisationFailed { get; private set; } = null!;
    public Event<PaymentCaptured> PaymentCaptured { get; private set; } = null!;
    public Event<PaymentCaptureFailed> PaymentCaptureFailed { get; private set; } = null!;
    public Event<PaymentReleased> PaymentReleased { get; private set; } = null!;
    public Event<PaymentRefunded> PaymentRefunded { get; private set; } = null!;

    public Event<OutboundFlightReserved> OutboundFlightReserved { get; private set; } = null!;
    public Event<ReturnFlightReserved> ReturnFlightReserved { get; private set; } = null!;
    public Event<FlightReservationFailed> FlightReservationFailed { get; private set; } = null!;
    public Event<FlightCancelled> FlightCancelled { get; private set; } = null!;

    public Event<HotelReserved> HotelReserved { get; private set; } = null!;
    public Event<HotelConfirmed> HotelConfirmed { get; private set; } = null!;
    public Event<HotelConfirmationExpired> HotelConfirmationExpired { get; private set; } = null!;
    public Event<HotelReservationFailed> HotelReservationFailed { get; private set; } = null!;
    public Event<HotelCancelled> HotelCancelled { get; private set; } = null!;

    public Event<GroundTransportReserved> GroundTransportReserved { get; private set; } = null!;
    public Event<GroundTransportReservationFailed> GroundTransportReservationFailed { get; private set; } = null!;
    public Event<GroundTransportCancelled> GroundTransportCancelled { get; private set; } = null!;

    public Event<InsuranceIssued> InsuranceIssued { get; private set; } = null!;
    public Event<InsuranceIssueFailed> InsuranceIssueFailed { get; private set; } = null!;
    public Event<InsuranceCancelled> InsuranceCancelled { get; private set; } = null!;

    public Event<NotificationSent> NotificationSent { get; private set; } = null!;


    public TripBookingStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => TripBookingCreated,
            x => x.CorrelateById(context => context.Message.TripId)
                .SelectId(context => context.Message.TripId));

        Event(() => PaymentAuthorised,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => OutboundFlightReserved,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => ReturnFlightReserved,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => HotelReserved,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => HotelConfirmed,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => GroundTransportReserved,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => InsuranceIssued,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => PaymentCaptured,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => TripBookingCompleted,
            x => x.CorrelateById(context => context.Message.TripId));

        Event(() => TripBookingFailed,
            x => x.CorrelateById(context => context.Message.TripId));

        Event(() => TripBookingCancelled,
            x => x.CorrelateById(context => context.Message.TripId));

        Event(() => PaymentAuthorisationFailed,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => PaymentCaptureFailed,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => PaymentReleased,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => PaymentRefunded,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => FlightReservationFailed,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => FlightCancelled,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => HotelConfirmationExpired,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => HotelReservationFailed,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => HotelCancelled,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => GroundTransportReservationFailed,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => GroundTransportCancelled,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => InsuranceIssueFailed,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => InsuranceCancelled,
            x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => NotificationSent,
            x => x.CorrelateById(context => context.Message.TripId));

        Initially(
            When(TripBookingCreated)
                .Then(context =>
                {
                    var message = context.Message;
                    var saga = context.Saga;

                    saga.TripId = message.TripId;
                    saga.CustomerId = message.CustomerId;
                    saga.CustomerName = message.CustomerName;
                    saga.CustomerEmail = message.CustomerEmail;
                    saga.Origin = message.Origin;
                    saga.Destination = message.Destination;
                    saga.DepartureDate = message.DepartureDate;
                    saga.ReturnDate = message.ReturnDate;
                    saga.OutboundFlightNumber = message.OutboundFlightNumber;
                    saga.OutboundCarrier = message.OutboundCarrier;
                    saga.ReturnFlightNumber = message.ReturnFlightNumber;
                    saga.ReturnCarrier = message.ReturnCarrier;
                    saga.HotelId = message.HotelId;
                    saga.HotelName = message.HotelName;
                    saga.CheckIn = message.CheckIn;
                    saga.CheckOut = message.CheckOut;
                    saga.NumberOfGuests = message.NumberOfGuests;
                    saga.IncludeGroundTransport = message.IncludeGroundTransport;
                    saga.GroundTransportType = message.GroundTransportType;
                    saga.GroundTransportPickupLocation = message.GroundTransportPickupLocation;
                    saga.GroundTransportDropoffLocation = message.GroundTransportDropoffLocation;
                    saga.IncludeInsurance = message.IncludeInsurance;
                    saga.PaymentMethodId = message.PaymentMethodId;
                    saga.TotalAmount = message.TotalAmount;
                    saga.Currency = message.Currency;
                    saga.CreatedAt = message.CreatedAt;
                })
                .TransitionTo(AwaitingPaymentAuthorisation)
                .Publish(context => new AuthorisePayment(
                    CorrelationId: context.Saga.CorrelationId,
                    TripId: context.Saga.TripId,
                    CustomerId: context.Saga.CustomerId,
                    PaymentMethodId: context.Saga.PaymentMethodId,
                    Amount: context.Saga.TotalAmount,
                    Currency: context.Saga.Currency))
        );

        During(AwaitingPaymentAuthorisation,
            When(PaymentAuthorised)
                .Then(context =>
                {
                    context.Saga.PaymentTransactionId = context.Message.PaymentAuthorisationId;
                    context.Saga.IsPaymentAuthorised = true;
                    context.Saga.Currency = context.Message.Currency;
                    context.Saga.TotalAmount = context.Message.Amount;
                })
                .TransitionTo(AwaitingOutboundFlight)
                .Publish(context => new ReserveOutboundFlight(
                    CorrelationId: context.Saga.CorrelationId,
                    TripId: context.Saga.TripId,
                    Origin: context.Saga.Origin,
                    Destination: context.Saga.Destination,
                    DepartureDate: context.Saga.DepartureDate,
                    FlightNumber: context.Saga.OutboundFlightNumber,
                    Carrier: context.Saga.OutboundCarrier,
                    PassengerCount: context.Saga.NumberOfGuests)),
            When(PaymentAuthorisationFailed)
                .Then(context =>
                {
                    context.Saga.IsPaymentAuthorised = false;
                })
                .TransitionTo(Failed)
                .Publish(context => new TripBookingFailed(
                    TripId: context.Saga.TripId,
                    Reason: context.Message.Reason,
                    FailedAt: DateTime.UtcNow))
        );

        During(AwaitingOutboundFlight,
            When(OutboundFlightReserved)
                .Then(context =>
                {
                    context.Saga.OutboundFlightId = context.Message.FlightReservationId;
                    context.Saga.IsOutboundFlightReserved = true;
                })
                .TransitionTo(AwaitingReturnFlight)
                .Publish(context => new ReserveReturnFlight(
                    CorrelationId: context.Saga.CorrelationId,
                    TripId: context.Saga.TripId,
                    Origin: context.Saga.Destination,
                    Destination: context.Saga.Origin,
                    DepartureDate: context.Saga.ReturnDate,
                    FlightNumber: context.Saga.ReturnFlightNumber,
                    Carrier: context.Saga.ReturnCarrier,
                    PassengerCount: context.Saga.NumberOfGuests)),
            When(FlightReservationFailed)
                .Then(context =>
                {
                    context.Saga.IsOutboundFlightReserved = false;
                })
                .TransitionTo(ReleasingPayment)
                .Publish(context => new ReleasePayment(
                    CorrelationId: context.Saga.CorrelationId,
                    TripId: context.Saga.TripId,
                    PaymentAuthorisationId: context.Saga.PaymentTransactionId!.Value,
                    Reason: context.Message.Reason))
        );

        // Compensation: Outbound flight cancelled - release payment
        During(CompensatingOutboundFlight,
            When(FlightCancelled)
                .Then(context => context.Saga.IsOutboundFlightReserved = false)
                .TransitionTo(ReleasingPayment)
                .Publish(context => new ReleasePayment(
                    CorrelationId: context.Saga.CorrelationId,
                    TripId: context.Saga.TripId,
                    PaymentAuthorisationId: context.Saga.PaymentTransactionId!.Value,
                    Reason: context.Message.Reason))
        );

        During(AwaitingReturnFlight,
            When(ReturnFlightReserved)
                .Then(context =>
                {
                    context.Saga.ReturnFlightId = context.Message.FlightReservationId;
                    context.Saga.IsReturnFlightReserved = true;
                })
                .TransitionTo(AwaitingHotel)
                .Publish(context => new ReserveHotel(
                    CorrelationId: context.Saga.CorrelationId,
                    TripId: context.Saga.TripId,
                    HotelId: context.Saga.HotelId,
                    HotelName: context.Saga.HotelName,
                    CheckIn: context.Saga.CheckIn,
                    CheckOut: context.Saga.CheckOut,
                    NumberOfGuests: context.Saga.NumberOfGuests,
                    GuestName: context.Saga.CustomerName,
                    GuestEmail: context.Saga.CustomerEmail)),
            When(FlightReservationFailed)
                .Then(context =>
                {
                    context.Saga.IsReturnFlightReserved = false;
                })
                .TransitionTo(CompensatingOutboundFlight)
                .Publish(context => new CancelFlight(
                    CorrelationId: context.Saga.CorrelationId,
                    TripId: context.Saga.TripId,
                    FlightReservationId: context.Saga.OutboundFlightId!.Value,
                    Reason: context.Message.Reason))
        );

        // Compensation: Return flight cancelled - continue to outbound flight
        During(CompensatingReturnFlight,
            When(FlightCancelled)
                .Then(context => context.Saga.IsReturnFlightReserved = false)
                .TransitionTo(CompensatingOutboundFlight)
                .Publish(context => new CancelFlight(
                    CorrelationId: context.Saga.CorrelationId,
                    TripId: context.Saga.TripId,
                    FlightReservationId: context.Saga.OutboundFlightId!.Value,
                    Reason: context.Message.Reason))
        );

        During(AwaitingHotel,
            When(HotelReserved)
                .Then(context =>
                {
                    context.Saga.HotelReservationId = context.Message.HotelReservationId;
                    context.Saga.IsHotelReserved = true;
                })
                .TransitionTo(AwaitingHotelConfirmation)
                .Publish(context => new ConfirmHotel(
                    CorrelationId: context.Saga.CorrelationId,
                    TripId: context.Saga.TripId,
                    HotelReservationId: context.Saga.HotelReservationId!.Value)),
            When(HotelReservationFailed)
                .Then(context =>
                {
                    context.Saga.IsHotelReserved = false;
                })
                .TransitionTo(CompensatingReturnFlight)
                .Publish(context => new CancelFlight(
                    CorrelationId: context.Saga.CorrelationId,
                    TripId: context.Saga.TripId,
                    FlightReservationId: context.Saga.ReturnFlightId!.Value,
                    Reason: context.Message.Reason))
        );

        During(AwaitingHotelConfirmation,
            When(HotelConfirmationExpired)
                .Then(context =>
                {
                    context.Saga.IsHotelReserved = false;
                })
                .TransitionTo(CompensatingHotel)
                .Publish(context => new CancelHotel(
                    CorrelationId: context.Saga.CorrelationId,
                    TripId: context.Saga.TripId,
                    HotelReservationId: context.Saga.HotelReservationId!.Value,
                    Reason: "Hotel confirmation expired"))
        );

        // Compensation: Hotel cancelled - continue to return flight
        During(CompensatingHotel,
            When(HotelCancelled)
                .Then(context => context.Saga.IsHotelReserved = false)
                .TransitionTo(CompensatingReturnFlight)
                .Publish(context => new CancelFlight(
                    CorrelationId: context.Saga.CorrelationId,
                    TripId: context.Saga.TripId,
                    FlightReservationId: context.Saga.ReturnFlightId!.Value,
                    Reason: context.Message.Reason))
        );

        // Hotel confirmed - branch to optional services or payment capture
        During(AwaitingHotelConfirmation,
            When(HotelConfirmed)
                .Then(context => context.Saga.IsHotelConfirmed = true)
                .IfElse(
                    context => context.Saga.IncludeGroundTransport,
                    // Branch: GroundTransport included
                    withTransport => withTransport
                        .TransitionTo(AwaitingGroundTransport)
                        .Publish(context => new ReserveGroundTransport(
                            CorrelationId: context.Saga.CorrelationId,
                            TripId: context.Saga.TripId,
                            Type: context.Saga.GroundTransportType!,
                            PickupLocation: context.Saga.GroundTransportPickupLocation!,
                            DropoffLocation: context.Saga.GroundTransportDropoffLocation!,
                            PickupDate: context.Saga.DepartureDate,
                            PassengerCount: context.Saga.NumberOfGuests)),
                    // Branch: No GroundTransport
                    noTransport => noTransport.IfElse(
                        context => context.Saga.IncludeInsurance,
                        // Sub-branch: Insurance included
                        withInsurance => withInsurance
                            .TransitionTo(AwaitingInsurance)
                            .Publish(context => new IssueInsurance(
                                CorrelationId: context.Saga.CorrelationId,
                                TripId: context.Saga.TripId,
                                CustomerId: context.Saga.CustomerId,
                                CustomerName: context.Saga.CustomerName,
                                CustomerEmail: context.Saga.CustomerEmail,
                                OutboundFlightReservationId: context.Saga.OutboundFlightId!.Value,
                                ReturnFlightReservationId: context.Saga.ReturnFlightId!.Value,
                                HotelReservationId: context.Saga.HotelReservationId!.Value,
                                CoverageStartDate: context.Saga.DepartureDate,
                                CoverageEndDate: context.Saga.ReturnDate,
                                TripTotalValue: context.Saga.TotalAmount)),
                        // Sub-branch: No Insurance - go directly to payment
                        noInsurance => noInsurance
                            .TransitionTo(AwaitingPaymentCapture)
                            .Publish(context => new CapturePayment(
                                CorrelationId: context.Saga.CorrelationId,
                                TripId: context.Saga.TripId,
                                PaymentAuthorisationId: context.Saga.PaymentTransactionId!.Value,
                                Amount: context.Saga.TotalAmount))))
        );

        // Ground transport reserved - branch to insurance or payment capture
        During(AwaitingGroundTransport,
            When(GroundTransportReserved)
                .Then(context =>
                {
                    context.Saga.GroundTransportId = context.Message.TransportReservationId;
                    context.Saga.IsGroundTransportReserved = true;
                })
                .IfElse(
                    context => context.Saga.IncludeInsurance,
                    // Branch: Insurance included
                    withInsurance => withInsurance
                        .TransitionTo(AwaitingInsurance)
                        .Publish(context => new IssueInsurance(
                            CorrelationId: context.Saga.CorrelationId,
                            TripId: context.Saga.TripId,
                            CustomerId: context.Saga.CustomerId,
                            CustomerName: context.Saga.CustomerName,
                            CustomerEmail: context.Saga.CustomerEmail,
                            OutboundFlightReservationId: context.Saga.OutboundFlightId!.Value,
                            ReturnFlightReservationId: context.Saga.ReturnFlightId!.Value,
                            HotelReservationId: context.Saga.HotelReservationId!.Value,
                            CoverageStartDate: context.Saga.DepartureDate,
                            CoverageEndDate: context.Saga.ReturnDate,
                            TripTotalValue: context.Saga.TotalAmount)),
                    // Branch: No Insurance - go directly to payment
                    noInsurance => noInsurance
                        .TransitionTo(AwaitingPaymentCapture)
                        .Publish(context => new CapturePayment(
                            CorrelationId: context.Saga.CorrelationId,
                            TripId: context.Saga.TripId,
                            PaymentAuthorisationId: context.Saga.PaymentTransactionId!.Value,
                            Amount: context.Saga.TotalAmount))),

            // Failure: compensate hotel
            When(GroundTransportReservationFailed)
                .Then(context => context.Saga.IsGroundTransportReserved = false)
                .TransitionTo(CompensatingHotel)
                .Publish(context => new CancelHotel(
                    CorrelationId: context.Saga.CorrelationId,
                    TripId: context.Saga.TripId,
                    HotelReservationId: context.Saga.HotelReservationId!.Value,
                    Reason: context.Message.Reason))
        );

        // Compensation: GroundTransport cancelled - continue to hotel
        During(CompensatingGroundTransport,
            When(GroundTransportCancelled)
                .Then(context => context.Saga.IsGroundTransportReserved = false)
                .TransitionTo(CompensatingHotel)
                .Publish(context => new CancelHotel(
                    CorrelationId: context.Saga.CorrelationId,
                    TripId: context.Saga.TripId,
                    HotelReservationId: context.Saga.HotelReservationId!.Value,
                    Reason: context.Message.Reason))
        );

        // Insurance issued - go to payment capture
        During(AwaitingInsurance,
            When(InsuranceIssued)
                .Then(context =>
                {
                    context.Saga.InsurancePolicyId = context.Message.InsurancePolicyId;
                    context.Saga.IsInsuranceIssued = true;
                })
                .TransitionTo(AwaitingPaymentCapture)
                .Publish(context => new CapturePayment(
                    CorrelationId: context.Saga.CorrelationId,
                    TripId: context.Saga.TripId,
                    PaymentAuthorisationId: context.Saga.PaymentTransactionId!.Value,
                    Amount: context.Saga.TotalAmount)),

            // Failure: compensate based on what was booked
            When(InsuranceIssueFailed)
                .Then(context => context.Saga.IsInsuranceIssued = false)
                .IfElse(
                    context => context.Saga.IncludeGroundTransport,
                    // Branch: Had GroundTransport - cancel it first
                    withTransport => withTransport
                        .TransitionTo(CompensatingGroundTransport)
                        .Publish(context => new CancelGroundTransport(
                            CorrelationId: context.Saga.CorrelationId,
                            TripId: context.Saga.TripId,
                            TransportReservationId: context.Saga.GroundTransportId!.Value,
                            Reason: context.Message.Reason)),
                    // Branch: No GroundTransport - cancel hotel directly
                    noTransport => noTransport
                        .TransitionTo(CompensatingHotel)
                        .Publish(context => new CancelHotel(
                            CorrelationId: context.Saga.CorrelationId,
                            TripId: context.Saga.TripId,
                            HotelReservationId: context.Saga.HotelReservationId!.Value,
                            Reason: context.Message.Reason)))
        );

        // Compensation: Insurance cancelled - continue compensation chain
        During(CompensatingInsurance,
            When(InsuranceCancelled)
                .Then(context => context.Saga.IsInsuranceIssued = false)
                .IfElse(
                    context => context.Saga.IncludeGroundTransport,
                    // Branch: Had GroundTransport - cancel it
                    withTransport => withTransport
                        .TransitionTo(CompensatingGroundTransport)
                        .Publish(context => new CancelGroundTransport(
                            CorrelationId: context.Saga.CorrelationId,
                            TripId: context.Saga.TripId,
                            TransportReservationId: context.Saga.GroundTransportId!.Value,
                            Reason: context.Message.Reason)),
                    // Branch: No GroundTransport - cancel hotel directly
                    noTransport => noTransport
                        .TransitionTo(CompensatingHotel)
                        .Publish(context => new CancelHotel(
                            CorrelationId: context.Saga.CorrelationId,
                            TripId: context.Saga.TripId,
                            HotelReservationId: context.Saga.HotelReservationId!.Value,
                            Reason: context.Message.Reason)))
        );


        // Payment capture - final step before completion
        During(AwaitingPaymentCapture,
            When(PaymentCaptured)
                .Then(context =>
                {
                    context.Saga.PaymentTransactionId = context.Message.PaymentId;
                    context.Saga.IsPaymentCaptured = true;
                })
                .TransitionTo(Completed)
                .Publish(context => new TripBookingCompleted(
                    TripId: context.Saga.TripId,
                    CompletedAt: DateTime.UtcNow)),

            // Failure: retry up to 3 times, then compensate
            When(PaymentCaptureFailed)
                .Then(context => context.Saga.PaymentCaptureRetryCount++)
                .IfElse(
                    context => context.Saga.PaymentCaptureRetryCount < 3,
                    // Retry: attempt payment capture again
                    retry => retry
                        .TransitionTo(AwaitingPaymentCapture)
                        .Publish(context => new CapturePayment(
                            CorrelationId: context.Saga.CorrelationId,
                            TripId: context.Saga.TripId,
                            PaymentAuthorisationId: context.Saga.PaymentTransactionId!.Value,
                            Amount: context.Saga.TotalAmount)),
                    // Exhausted retries: start compensation chain
                    exhausted => exhausted.IfElse(
                        context => context.Saga.IncludeInsurance,
                        // Branch: Had Insurance - cancel it first
                        withInsurance => withInsurance
                            .TransitionTo(CompensatingInsurance)
                            .Publish(context => new CancelInsurance(
                                CorrelationId: context.Saga.CorrelationId,
                                TripId: context.Saga.TripId,
                                InsurancePolicyId: context.Saga.InsurancePolicyId!.Value,
                                Reason: context.Message.Reason)),
                        // Branch: No Insurance
                        noInsurance => noInsurance.IfElse(
                            context => context.Saga.IncludeGroundTransport,
                            // Sub-branch: Had GroundTransport - cancel it
                            withTransport => withTransport
                                .TransitionTo(CompensatingGroundTransport)
                                .Publish(context => new CancelGroundTransport(
                                    CorrelationId: context.Saga.CorrelationId,
                                    TripId: context.Saga.TripId,
                                    TransportReservationId: context.Saga.GroundTransportId!.Value,
                                    Reason: context.Message.Reason)),
                            // Sub-branch: No GroundTransport - cancel hotel directly
                            noTransport => noTransport
                                .TransitionTo(CompensatingHotel)
                                .Publish(context => new CancelHotel(
                                    CorrelationId: context.Saga.CorrelationId,
                                    TripId: context.Saga.TripId,
                                    HotelReservationId: context.Saga.HotelReservationId!.Value,
                                    Reason: context.Message.Reason)))))
        );


        // Final compensation step: Payment released - transition to Failed
        During(ReleasingPayment,
            When(PaymentReleased)
                .Then(context => context.Saga.PaymentTransactionId = context.Message.PaymentAuthorisationId)
                .TransitionTo(Failed)
                .Publish(context => new TripBookingFailed(
                    TripId: context.Saga.TripId,
                    Reason: context.Message.Reason,
                    FailedAt: DateTime.UtcNow))
        );

        // Ignore events that should not trigger any action in terminal states
        During(Completed,
            Ignore(TripBookingCompleted),
            Ignore(TripBookingFailed),
            Ignore(TripBookingCancelled));

        During(Failed,
            Ignore(TripBookingFailed),
            Ignore(TripBookingCancelled));
    }
}
