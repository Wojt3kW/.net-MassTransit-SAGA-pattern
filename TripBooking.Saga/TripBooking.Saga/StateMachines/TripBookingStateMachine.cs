using FlightBooking.Contracts.Commands;
using FlightBooking.Contracts.Events;
using GroundTransport.Contracts.Commands;
using GroundTransport.Contracts.Events;
using HotelBooking.Contracts.Commands;
using HotelBooking.Contracts.Events;
using Insurance.Contracts.Commands;
using Insurance.Contracts.Events;
using MassTransit;
using Payment.Contracts.Commands;
using Payment.Contracts.Events;
using Trip.Contracts.Events;
using TripBooking.Saga.States;

namespace TripBooking.Saga.StateMachines;

/// <summary>
/// Orchestrates the trip booking process using the Saga pattern.
/// Manages the complete booking lifecycle including payment, flights, hotel,
/// ground transport, insurance, and compensation on failures.
/// </summary>
public class TripBookingStateMachine : MassTransitStateMachine<TripBookingSagaState>
{
    private readonly TripBookingStateMachineSettings _settings;

    // Happy path states
    public State AwaitingPaymentAuthorisation { get; private set; } = null!;
    public State AwaitingOutboundFlight { get; private set; } = null!;
    public State AwaitingReturnFlight { get; private set; } = null!;
    public State AwaitingHotel { get; private set; } = null!;
    public State AwaitingHotelConfirmation { get; private set; } = null!;
    public State AwaitingGroundTransport { get; private set; } = null!;
    public State AwaitingInsurance { get; private set; } = null!;
    public State AwaitingPaymentCapture { get; private set; } = null!;
    public State AwaitingPaymentRefund { get; private set; } = null!;
    public State Completed { get; private set; } = null!;

    // Compensation states
    public State CompensatingInsurance { get; private set; } = null!;
    public State CompensatingGroundTransport { get; private set; } = null!;
    public State CompensatingHotel { get; private set; } = null!;
    public State CompensatingReturnFlight { get; private set; } = null!;
    public State CompensatingOutboundFlight { get; private set; } = null!;
    public State ReleasingPayment { get; private set; } = null!;

    // Terminal states
    public State Failed { get; private set; } = null!;
    public State TimedOut { get; private set; } = null!;
    public State Cancelled { get; private set; } = null!;
    public State Refunded { get; private set; } = null!;

    // Trip lifecycle events
    public Event<TripBookingCreated> TripBookingCreated { get; private set; } = null!;
    public Event<TripBookingCompleted> TripBookingCompleted { get; private set; } = null!;
    public Event<TripBookingFailed> TripBookingFailed { get; private set; } = null!;
    public Event<TripBookingCancelled> TripBookingCancelled { get; private set; } = null!;
    public Event<TripRefundRequested> TripRefundRequested { get; private set; } = null!;
    public Event<TripBookingRefunded> TripBookingRefunded { get; private set; } = null!;

    // Payment events
    public Event<PaymentAuthorised> PaymentAuthorised { get; private set; } = null!;
    public Event<PaymentAuthorisationFailed> PaymentAuthorisationFailed { get; private set; } = null!;
    public Event<PaymentCaptured> PaymentCaptured { get; private set; } = null!;
    public Event<PaymentCaptureFailed> PaymentCaptureFailed { get; private set; } = null!;
    public Event<PaymentReleased> PaymentReleased { get; private set; } = null!;
    public Event<PaymentRefunded> PaymentRefunded { get; private set; } = null!;

    // Flight events
    public Event<OutboundFlightReserved> OutboundFlightReserved { get; private set; } = null!;
    public Event<ReturnFlightReserved> ReturnFlightReserved { get; private set; } = null!;
    public Event<FlightReservationFailed> FlightReservationFailed { get; private set; } = null!;
    public Event<FlightCancelled> FlightCancelled { get; private set; } = null!;

    // Hotel events
    public Event<HotelReserved> HotelReserved { get; private set; } = null!;
    public Event<HotelConfirmed> HotelConfirmed { get; private set; } = null!;
    public Event<HotelConfirmationExpired> HotelConfirmationExpired { get; private set; } = null!;
    public Event<HotelReservationFailed> HotelReservationFailed { get; private set; } = null!;
    public Event<HotelCancelled> HotelCancelled { get; private set; } = null!;

    // Ground transport events
    public Event<GroundTransportReserved> GroundTransportReserved { get; private set; } = null!;
    public Event<GroundTransportReservationFailed> GroundTransportReservationFailed { get; private set; } = null!;
    public Event<GroundTransportCancelled> GroundTransportCancelled { get; private set; } = null!;

    // Insurance events
    public Event<InsuranceIssued> InsuranceIssued { get; private set; } = null!;
    public Event<InsuranceIssueFailed> InsuranceIssueFailed { get; private set; } = null!;
    public Event<InsuranceCancelled> InsuranceCancelled { get; private set; } = null!;

    // Schedules
    public Schedule<TripBookingSagaState, PaymentAuthorisationTimedOut> PaymentAuthorisationTimeoutSchedule { get; private set; } = null!;
    public Schedule<TripBookingSagaState, OutboundFlightReservationTimedOut> OutboundFlightReservationTimeoutSchedule { get; private set; } = null!;
    public Schedule<TripBookingSagaState, ReturnFlightReservationTimedOut> ReturnFlightReservationTimeoutSchedule { get; private set; } = null!;
    public Schedule<TripBookingSagaState, HotelReservationTimedOut> HotelReservationTimeoutSchedule { get; private set; } = null!;
    public Schedule<TripBookingSagaState, HotelConfirmationTimedOut> HotelConfirmationTimeoutSchedule { get; private set; } = null!;
    public Schedule<TripBookingSagaState, GroundTransportReservationTimedOut> GroundTransportReservationTimeoutSchedule { get; private set; } = null!;
    public Schedule<TripBookingSagaState, InsuranceIssuingTimedOut> InsuranceIssuingTimeoutSchedule { get; private set; } = null!;
    public Schedule<TripBookingSagaState, PaymentCaptureTimedOut> PaymentCaptureTimeoutSchedule { get; private set; } = null!;

    public TripBookingStateMachine()
        : this(new TripBookingStateMachineSettings())
    {
    }

    public TripBookingStateMachine(TripBookingStateMachineSettings settings)
    {
        _settings = settings;

        InstanceState(x => x.CurrentState);

        ConfigureEvents();
        ConfigureSchedules();

        ConfigureInitialState();
        ConfigurePaymentAuthorisationState();
        ConfigureFlightReservationStates();
        ConfigureHotelReservationStates();
        ConfigureGroundTransportState();
        ConfigureInsuranceState();
        ConfigurePaymentCaptureState();
        ConfigurePaymentRefundState();
        ConfigureCompensationStates();
        ConfigureTerminalStates();
    }

    private void ConfigureEvents()
    {
        // Trip lifecycle events
        Event(() => TripBookingCreated, x => x
            .CorrelateById(context => context.Message.TripId)
            .SelectId(context => context.Message.TripId));

        Event(() => TripBookingCompleted, x => x.CorrelateById(context => context.Message.TripId));
        Event(() => TripBookingFailed, x => x.CorrelateById(context => context.Message.TripId));
        Event(() => TripBookingCancelled, x => x.CorrelateById(context => context.Message.TripId));
        Event(() => TripRefundRequested, x => x.CorrelateById(context => context.Message.TripId));
        Event(() => TripBookingRefunded, x => x.CorrelateById(context => context.Message.TripId));

        // Payment events - correlate by CorrelationId
        Event(() => PaymentAuthorised, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentAuthorisationFailed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentCaptured, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentCaptureFailed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentReleased, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentRefunded, x => x.CorrelateById(context => context.Message.CorrelationId));

        // Flight events
        Event(() => OutboundFlightReserved, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => ReturnFlightReserved, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => FlightReservationFailed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => FlightCancelled, x => x.CorrelateById(context => context.Message.CorrelationId));

        // Hotel events
        Event(() => HotelReserved, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => HotelConfirmed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => HotelConfirmationExpired, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => HotelReservationFailed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => HotelCancelled, x => x.CorrelateById(context => context.Message.CorrelationId));

        // Ground transport events
        Event(() => GroundTransportReserved, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => GroundTransportReservationFailed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => GroundTransportCancelled, x => x.CorrelateById(context => context.Message.CorrelationId));

        // Insurance events
        Event(() => InsuranceIssued, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => InsuranceIssueFailed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => InsuranceCancelled, x => x.CorrelateById(context => context.Message.CorrelationId));
    }

    private void ConfigureSchedules()
    {
        Schedule(() => PaymentAuthorisationTimeoutSchedule,
            saga => saga.PaymentAuthorisationTimeoutToken,
            schedule =>
            {
                schedule.Delay = _settings.PaymentAuthorisationTimeout;
                schedule.Received = r => r.CorrelateById(m => m.Message.TripId);
            });

        Schedule(() => OutboundFlightReservationTimeoutSchedule,
            saga => saga.OutboundFlightTimeoutToken,
            schedule =>
            {
                schedule.Delay = _settings.OutboundFlightReservationTimeout;
                schedule.Received = r => r.CorrelateById(m => m.Message.TripId);
            });

        Schedule(() => ReturnFlightReservationTimeoutSchedule,
            saga => saga.ReturnFlightTimeoutToken,
            schedule =>
            {
                schedule.Delay = _settings.ReturnFlightReservationTimeout;
                schedule.Received = r => r.CorrelateById(m => m.Message.TripId);
            });

        Schedule(() => HotelReservationTimeoutSchedule,
            saga => saga.HotelReservationTimeoutToken,
            schedule =>
            {
                schedule.Delay = _settings.HotelReservationTimeout;
                schedule.Received = r => r.CorrelateById(m => m.Message.TripId);
            });

        Schedule(() => HotelConfirmationTimeoutSchedule,
            saga => saga.HotelConfirmationTimeoutToken,
            schedule =>
            {
                schedule.Delay = _settings.HotelConfirmationTimeout;
                schedule.Received = r => r.CorrelateById(m => m.Message.TripId);
            });

        Schedule(() => GroundTransportReservationTimeoutSchedule,
            saga => saga.GroundTransportTimeoutToken,
            schedule =>
            {
                schedule.Delay = _settings.GroundTransportReservationTimeout;
                schedule.Received = r => r.CorrelateById(m => m.Message.TripId);
            });

        Schedule(() => InsuranceIssuingTimeoutSchedule,
            saga => saga.InsuranceTimeoutToken,
            schedule =>
            {
                schedule.Delay = _settings.InsuranceIssuingTimeout;
                schedule.Received = r => r.CorrelateById(m => m.Message.TripId);
            });

        Schedule(() => PaymentCaptureTimeoutSchedule,
            saga => saga.PaymentCaptureTimeoutToken,
            schedule =>
            {
                schedule.Delay = _settings.PaymentCaptureTimeout;
                schedule.Received = r => r.CorrelateById(m => m.Message.TripId);
            });
    }

    private void ConfigureInitialState()
    {
        Initially(
            When(TripBookingCreated)
                .Then(context => InitializeSagaState(context.Saga, context.Message))
                .TransitionTo(AwaitingPaymentAuthorisation)
                .Schedule(PaymentAuthorisationTimeoutSchedule, context =>
                    new PaymentAuthorisationTimedOut(context.Saga.CorrelationId, context.Saga.TripId))
                .Publish(context => CreateAuthorisePaymentCommand(context.Saga))
        );
    }

    private void ConfigurePaymentAuthorisationState()
    {
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
                .Unschedule(PaymentAuthorisationTimeoutSchedule)
                .Schedule(OutboundFlightReservationTimeoutSchedule, context =>
                    new OutboundFlightReservationTimedOut(context.Saga.CorrelationId, context.Saga.TripId))
                .Publish(context => CreateReserveOutboundFlightCommand(context.Saga)),

            // Failure: transition to Failed state
            When(PaymentAuthorisationFailed)
                .Then(context =>
                {
                    context.Saga.IsPaymentAuthorised = false;
                    context.Saga.FailureReason = context.Message.Reason;
                })
                .TransitionTo(Failed)
                .Unschedule(PaymentAuthorisationTimeoutSchedule)
                .Publish(context => CreateTripBookingFailedEvent(context.Saga, context.Message.Reason)),

            // Timeout: transition to TimedOut state
            When(PaymentAuthorisationTimeoutSchedule.Received)
                .Then(context => context.Saga.FailureReason = $"Payment authorisation timed out after {_settings.PaymentAuthorisationTimeout.TotalSeconds} seconds")
                .TransitionTo(TimedOut)
                .Publish(context =>
                    CreateTripBookingFailedEvent(
                        context.Saga,
                        $"Payment authorisation timed out after {_settings.PaymentAuthorisationTimeout.TotalSeconds} seconds")
                ),

            // Trip canceled - no resources reserved yet, just cancel
            When(TripBookingCancelled)
                .Then(context => context.Saga.IsCancelledByUser = true)
                .Unschedule(PaymentAuthorisationTimeoutSchedule)
                .TransitionTo(Cancelled)
        );
    }

    private void ConfigureFlightReservationStates()
    {
        // Outbound flight reservation
        During(AwaitingOutboundFlight,

            When(OutboundFlightReserved)
                .Then(context =>
                {
                    context.Saga.OutboundFlightId = context.Message.FlightReservationId;
                    context.Saga.IsOutboundFlightReserved = true;
                })
                .TransitionTo(AwaitingReturnFlight)
                .Unschedule(OutboundFlightReservationTimeoutSchedule)
                .Schedule(ReturnFlightReservationTimeoutSchedule, context =>
                    new ReturnFlightReservationTimedOut(context.Saga.CorrelationId, context.Saga.TripId))
                .Publish(context => CreateReserveReturnFlightCommand(context.Saga)),

            When(FlightReservationFailed)
                .Then(context =>
                {
                    context.Saga.IsOutboundFlightReserved = false;
                    context.Saga.FailureReason = context.Message.Reason;
                })
                .TransitionTo(ReleasingPayment)
                .Unschedule(OutboundFlightReservationTimeoutSchedule)
                .Publish(context => CreateReleasePaymentCommand(context.Saga, context.Message.Reason)),

            When(OutboundFlightReservationTimeoutSchedule.Received)
                .Then(context =>
                {
                    context.Saga.IsOutboundFlightReserved = false;
                    context.Saga.FailureReason = $"Outbound flight reservation timed out after {_settings.OutboundFlightReservationTimeout.TotalSeconds} seconds";
                })
                .TransitionTo(ReleasingPayment)
                .Publish(context => CreateReleasePaymentCommand(context.Saga, $"Outbound flight reservation timed out after {_settings.OutboundFlightReservationTimeout.TotalSeconds} seconds")),

            When(TripBookingCancelled)
                .Then(context => context.Saga.IsCancelledByUser = true)
                .Unschedule(OutboundFlightReservationTimeoutSchedule)
                .TransitionTo(ReleasingPayment)
                .Publish(context => CreateReleasePaymentCommand(context.Saga, context.Message.Reason))
        );

        During(AwaitingReturnFlight,
            When(ReturnFlightReserved)
                .Then(context =>
                {
                    context.Saga.ReturnFlightId = context.Message.FlightReservationId;
                    context.Saga.IsReturnFlightReserved = true;
                })
                .TransitionTo(AwaitingHotel)
                .Unschedule(ReturnFlightReservationTimeoutSchedule)
                .Schedule(HotelReservationTimeoutSchedule, context =>
                    new HotelReservationTimedOut(context.Saga.CorrelationId, context.Saga.TripId))
                .Publish(context => CreateReserveHotelCommand(context.Saga)),

            When(FlightReservationFailed)
                .Then(context =>
                {
                    context.Saga.IsReturnFlightReserved = false;
                    context.Saga.FailureReason = context.Message.Reason;
                })
                .TransitionTo(CompensatingOutboundFlight)
                .Unschedule(ReturnFlightReservationTimeoutSchedule)
                .Publish(context => CreateCancelOutboundFlightCommand(context.Saga, context.Message.Reason)),

            When(ReturnFlightReservationTimeoutSchedule.Received)
                .Then(context =>
                {
                    context.Saga.IsReturnFlightReserved = false;
                    context.Saga.FailureReason = $"Return flight reservation timed out after {_settings.ReturnFlightReservationTimeout.TotalSeconds} seconds";
                })
                .TransitionTo(CompensatingOutboundFlight)
                .Publish(context =>
                    CreateCancelOutboundFlightCommand(
                        context.Saga,
                        $"Return flight reservation timed out after {_settings.ReturnFlightReservationTimeout.TotalSeconds} seconds")
                ),

            When(TripBookingCancelled)
                .Then(context => context.Saga.IsCancelledByUser = true)
                .Unschedule(ReturnFlightReservationTimeoutSchedule)
                .TransitionTo(CompensatingOutboundFlight)
                .Publish(context => CreateCancelOutboundFlightCommand(context.Saga, context.Message.Reason))
        );
    }

    private void ConfigureHotelReservationStates()
    {
        During(AwaitingHotel,
            When(HotelReserved)
                .Then(context =>
                {
                    context.Saga.HotelReservationId = context.Message.HotelReservationId;
                    context.Saga.IsHotelReserved = true;
                })
                .TransitionTo(AwaitingHotelConfirmation)
                .Unschedule(HotelReservationTimeoutSchedule)
                .Schedule(HotelConfirmationTimeoutSchedule, context =>
                    new HotelConfirmationTimedOut(context.Saga.CorrelationId, context.Saga.TripId, context.Saga.HotelReservationId))
                .Publish(context => CreateConfirmHotelCommand(context.Saga)),

            When(HotelReservationFailed)
                .Then(context =>
                {
                    context.Saga.IsHotelReserved = false;
                    context.Saga.FailureReason = context.Message.Reason;
                })
                .TransitionTo(CompensatingReturnFlight)
                .Unschedule(HotelReservationTimeoutSchedule)
                .Publish(context => CreateCancelReturnFlightCommand(context.Saga, context.Message.Reason)),

            When(HotelReservationTimeoutSchedule.Received)
                .Then(context =>
                {
                    context.Saga.IsHotelReserved = false;
                    context.Saga.FailureReason = $"Hotel reservation timed out after {_settings.HotelReservationTimeout.TotalSeconds} seconds";
                })
                .TransitionTo(CompensatingReturnFlight)
                .Publish(context =>
                    CreateCancelReturnFlightCommand(
                        context.Saga,
                        $"Hotel reservation timed out after {_settings.HotelReservationTimeout.TotalSeconds} seconds")
                ),

            When(TripBookingCancelled)
                .Then(context => context.Saga.IsCancelledByUser = true)
                .Unschedule(HotelReservationTimeoutSchedule)
                .TransitionTo(CompensatingReturnFlight)
                .Publish(context => CreateCancelReturnFlightCommand(context.Saga, context.Message.Reason))
        );

        During(AwaitingHotelConfirmation,
            When(HotelConfirmed)
                .Then(context => context.Saga.IsHotelConfirmed = true)
                .Unschedule(HotelConfirmationTimeoutSchedule)
                .IfElse(
                    context => context.Saga.IncludeGroundTransport,
                    withTransport => withTransport
                        .TransitionTo(AwaitingGroundTransport)
                        .Schedule(GroundTransportReservationTimeoutSchedule, context =>
                            new GroundTransportReservationTimedOut(context.Saga.CorrelationId, context.Saga.TripId))
                        .Publish(context => CreateReserveGroundTransportCommand(context.Saga)),
                    noTransport => noTransport.IfElse(
                        context => context.Saga.IncludeInsurance,
                        withInsurance => withInsurance
                            .TransitionTo(AwaitingInsurance)
                            .Schedule(InsuranceIssuingTimeoutSchedule, context =>
                                new InsuranceIssuingTimedOut(context.Saga.CorrelationId, context.Saga.TripId))
                            .Publish(context => CreateIssueInsuranceCommand(context.Saga)),
                        noInsurance => noInsurance
                            .TransitionTo(AwaitingPaymentCapture)
                            .Schedule(PaymentCaptureTimeoutSchedule, context =>
                                new PaymentCaptureTimedOut(context.Saga.CorrelationId, context.Saga.TripId, context.Saga.PaymentTransactionId))
                            .Publish(context => CreateCapturePaymentCommand(context.Saga)))),

            When(HotelConfirmationExpired)
                .Then(context => context.Saga.IsHotelReserved = false)
                .TransitionTo(CompensatingHotel)
                .Unschedule(HotelConfirmationTimeoutSchedule)
                .Publish(context =>
                    CreateCancelHotelCommand(
                        context.Saga,
                        "Hotel confirmation expired")),

            When(HotelConfirmationTimeoutSchedule.Received)
                .Then(context =>
                {
                    context.Saga.IsHotelReserved = false;
                    context.Saga.FailureReason = $"Hotel confirmation timed out after {_settings.HotelConfirmationTimeout.TotalSeconds} seconds";
                })
                .TransitionTo(CompensatingHotel)
                .Publish(context =>
                    CreateCancelHotelCommand(
                        context.Saga,
                        $"Hotel confirmation timed out after {_settings.HotelConfirmationTimeout.TotalSeconds} seconds")
                ),


            When(TripBookingCancelled)
                .Then(context => context.Saga.IsCancelledByUser = true)
                .Unschedule(HotelConfirmationTimeoutSchedule)
                .TransitionTo(CompensatingHotel)
                .Publish(context => CreateCancelHotelCommand(context.Saga, context.Message.Reason))
        );
    }

    private void ConfigureGroundTransportState()
    {
        During(AwaitingGroundTransport,
            When(GroundTransportReserved)
                .Then(context =>
                {
                    context.Saga.GroundTransportId = context.Message.TransportReservationId;
                    context.Saga.IsGroundTransportReserved = true;
                })
                .Unschedule(GroundTransportReservationTimeoutSchedule)
                .IfElse(
                    context => context.Saga.IncludeInsurance,
                    withInsurance => withInsurance
                        .TransitionTo(AwaitingInsurance)
                        .Schedule(InsuranceIssuingTimeoutSchedule, context =>
                            new InsuranceIssuingTimedOut(context.Saga.CorrelationId, context.Saga.TripId))
                        .Publish(context => CreateIssueInsuranceCommand(context.Saga)),
                    noInsurance => noInsurance
                        .TransitionTo(AwaitingPaymentCapture)
                        .Schedule(PaymentCaptureTimeoutSchedule, context =>
                            new PaymentCaptureTimedOut(context.Saga.CorrelationId, context.Saga.TripId, context.Saga.PaymentTransactionId))
                        .Publish(context => CreateCapturePaymentCommand(context.Saga))),

            When(GroundTransportReservationFailed)
                .Then(context =>
                {
                    context.Saga.IsGroundTransportReserved = false;
                    context.Saga.FailureReason = context.Message.Reason;
                })
                .TransitionTo(CompensatingHotel)
                .Unschedule(GroundTransportReservationTimeoutSchedule)
                .Publish(context => CreateCancelHotelCommand(context.Saga, context.Message.Reason)),

            When(GroundTransportReservationTimeoutSchedule.Received)
                .Then(context =>
                {
                    context.Saga.IsGroundTransportReserved = false;
                    context.Saga.FailureReason = $"Ground transport reservation timed out after {_settings.GroundTransportReservationTimeout.TotalSeconds} seconds";
                })
                .TransitionTo(CompensatingHotel)
                .Publish(context =>
                    CreateCancelHotelCommand(
                        context.Saga,
                        $"Ground transport reservation timed out after {_settings.GroundTransportReservationTimeout.TotalSeconds} seconds")
                ),

            When(TripBookingCancelled)
                .Then(context => context.Saga.IsCancelledByUser = true)
                .Unschedule(GroundTransportReservationTimeoutSchedule)
                .TransitionTo(CompensatingHotel)
                .Publish(context => CreateCancelHotelCommand(context.Saga, context.Message.Reason))
        );
    }

    private void ConfigureInsuranceState()
    {
        During(AwaitingInsurance,
            When(InsuranceIssued)
                .Then(context =>
                {
                    context.Saga.InsurancePolicyId = context.Message.InsurancePolicyId;
                    context.Saga.IsInsuranceIssued = true;
                })
                .Unschedule(InsuranceIssuingTimeoutSchedule)
                .Schedule(PaymentCaptureTimeoutSchedule, context =>
                    new PaymentCaptureTimedOut(context.Saga.CorrelationId, context.Saga.TripId, context.Saga.PaymentTransactionId))
                .TransitionTo(AwaitingPaymentCapture)
                .Publish(context => CreateCapturePaymentCommand(context.Saga)),

            When(InsuranceIssueFailed)
                .Then(context =>
                {
                    context.Saga.IsInsuranceIssued = false;
                    context.Saga.FailureReason = context.Message.Reason;
                })
                .Unschedule(InsuranceIssuingTimeoutSchedule)
                .IfElse(
                    context => context.Saga.IncludeGroundTransport,
                    withTransport => withTransport
                        .TransitionTo(CompensatingGroundTransport)
                        .Publish(context => CreateCancelGroundTransportCommand(context.Saga, context.Message.Reason)),
                    noTransport => noTransport
                        .TransitionTo(CompensatingHotel)
                        .Publish(context => CreateCancelHotelCommand(context.Saga, context.Message.Reason))),

            When(InsuranceIssuingTimeoutSchedule.Received)
                .Then(context =>
                {
                    context.Saga.IsInsuranceIssued = false;
                    context.Saga.FailureReason = $"Insurance issuing timed out after {_settings.InsuranceIssuingTimeout.TotalSeconds} seconds";
                })
                .IfElse(
                    context => context.Saga.IncludeGroundTransport,
                    withTransport => withTransport
                        .TransitionTo(CompensatingGroundTransport)
                        .Publish(context => CreateCancelGroundTransportCommand(context.Saga, $"Insurance issuing timed out after {_settings.InsuranceIssuingTimeout.TotalSeconds} seconds")),
                    noTransport => noTransport
                        .TransitionTo(CompensatingHotel)
                        .Publish(context => CreateCancelHotelCommand(context.Saga, $"Insurance issuing timed out after {_settings.InsuranceIssuingTimeout.TotalSeconds} seconds"))
                ),

            When(TripBookingCancelled)
                .Then(context => context.Saga.IsCancelledByUser = true)
                .Unschedule(InsuranceIssuingTimeoutSchedule)
                .IfElse(
                    context => context.Saga.IncludeGroundTransport,
                    withTransport => withTransport
                        .TransitionTo(CompensatingGroundTransport)
                        .Publish(context => CreateCancelGroundTransportCommand(context.Saga, context.Message.Reason)),
                    noTransport => noTransport
                        .TransitionTo(CompensatingHotel)
                        .Publish(context => CreateCancelHotelCommand(context.Saga, context.Message.Reason)))
        );
    }

    private void ConfigurePaymentCaptureState()
    {
        During(AwaitingPaymentCapture,
            When(PaymentCaptured)
                .Then(context =>
                {
                    context.Saga.PaymentTransactionId = context.Message.PaymentId;
                    context.Saga.IsPaymentCaptured = true;
                })
                .Unschedule(PaymentCaptureTimeoutSchedule)
                .TransitionTo(Completed)
                .Publish(context => CreateTripBookingCompletedEvent(context.Saga)),

            When(PaymentCaptureFailed)
                .Then(context => context.Saga.PaymentCaptureRetryCount++)
                .Unschedule(PaymentCaptureTimeoutSchedule)
                .IfElse(
                    context => context.Saga.PaymentCaptureRetryCount < _settings.PaymentCaptureMaxRetries,
                    retry => retry
                        .TransitionTo(AwaitingPaymentCapture)
                        .Schedule(PaymentCaptureTimeoutSchedule, context =>
                            new PaymentCaptureTimedOut(context.Saga.CorrelationId, context.Saga.TripId, context.Saga.PaymentTransactionId))
                        .Publish(context => CreateCapturePaymentCommand(context.Saga)),
                    exhausted => exhausted
                        .Then(context => context.Saga.FailureReason = context.Message.Reason)
                        .IfElse(
                        context => context.Saga.IncludeInsurance,
                        withInsurance => withInsurance
                            .TransitionTo(CompensatingInsurance)
                            .Publish(context => CreateCancelInsuranceCommand(context.Saga, context.Message.Reason)),
                        noInsurance => noInsurance.IfElse(
                            context => context.Saga.IncludeGroundTransport,
                            withTransport => withTransport
                                .TransitionTo(CompensatingGroundTransport)
                                .Publish(context => CreateCancelGroundTransportCommand(context.Saga, context.Message.Reason)),
                            noTransport => noTransport
                                .TransitionTo(CompensatingHotel)
                                .Publish(context => CreateCancelHotelCommand(context.Saga, context.Message.Reason))))
                ),

            When(PaymentCaptureTimeoutSchedule.Received)
                .Then(context =>
                {
                    context.Saga.IsPaymentCaptured = false;
                    context.Saga.FailureReason = $"Payment capture timed out after {_settings.PaymentCaptureTimeout.TotalSeconds} seconds";
                })
                .IfElse(
                    context => context.Saga.IncludeInsurance,
                    withInsurance => withInsurance
                        .TransitionTo(CompensatingInsurance)
                        .Publish(context => CreateCancelInsuranceCommand(context.Saga, $"Payment capture timed out after {_settings.PaymentCaptureTimeout.TotalSeconds} seconds")),
                    noInsurance => noInsurance.IfElse(
                        context => context.Saga.IncludeGroundTransport,
                        withTransport => withTransport
                            .TransitionTo(CompensatingGroundTransport)
                            .Publish(context => CreateCancelGroundTransportCommand(context.Saga, $"Payment capture timed out after {_settings.PaymentCaptureTimeout.TotalSeconds} seconds")),
                        noTransport => noTransport
                            .TransitionTo(CompensatingHotel)
                            .Publish(context => CreateCancelHotelCommand(context.Saga, $"Payment capture timed out after {_settings.PaymentCaptureTimeout.TotalSeconds} seconds")))
                ),


            When(TripBookingCancelled)
                .Then(context => context.Saga.IsCancelledByUser = true)
                .Unschedule(PaymentCaptureTimeoutSchedule)
                .IfElse(
                    context => context.Saga.IncludeInsurance,
                    withInsurance => withInsurance
                        .TransitionTo(CompensatingInsurance)
                        .Publish(context => CreateCancelInsuranceCommand(context.Saga, context.Message.Reason)),
                    noInsurance => noInsurance
                        .IfElse(
                            context => context.Saga.IncludeGroundTransport,
                            withTransport => withTransport
                                .TransitionTo(CompensatingGroundTransport)
                                .Publish(context => CreateCancelGroundTransportCommand(context.Saga, context.Message.Reason)),
                            noTransport => noTransport
                                .TransitionTo(CompensatingHotel)
                                .Publish(context => CreateCancelHotelCommand(context.Saga, context.Message.Reason))))
        );
    }

    private void ConfigurePaymentRefundState()
    {
        During(AwaitingPaymentRefund,
            When(PaymentRefunded)
                .Then(context =>
                {
                    context.Saga.IsRefunded = true;
                    context.Saga.CompletedAt = DateTime.UtcNow;
                })
                .TransitionTo(Refunded)
                .Publish(context => CreateTripBookingRefundedEvent(context.Saga))
        );
    }

    private void ConfigureCompensationStates()
    {
        During(CompensatingInsurance,
            When(InsuranceCancelled)
                .Then(context => context.Saga.IsInsuranceIssued = false)
                .IfElse(
                    context => context.Saga.IncludeGroundTransport,
                    withTransport => withTransport
                        .TransitionTo(CompensatingGroundTransport)
                        .Publish(context => CreateCancelGroundTransportCommand(context.Saga, context.Message.Reason)),
                    noTransport => noTransport
                        .TransitionTo(CompensatingHotel)
                        .Publish(context => CreateCancelHotelCommand(context.Saga, context.Message.Reason)))
        );

        During(CompensatingGroundTransport,
            When(GroundTransportCancelled)
                .Then(context => context.Saga.IsGroundTransportReserved = false)
                .TransitionTo(CompensatingHotel)
                .Publish(context => CreateCancelHotelCommand(context.Saga, context.Message.Reason))
        );

        During(CompensatingHotel,
            When(HotelCancelled)
                .Then(context => context.Saga.IsHotelReserved = false)
                .TransitionTo(CompensatingReturnFlight)
                .Publish(context => CreateCancelReturnFlightCommand(context.Saga, context.Message.Reason))
        );

        During(CompensatingReturnFlight,
            When(FlightCancelled)
                .Then(context => context.Saga.IsReturnFlightReserved = false)
                .TransitionTo(CompensatingOutboundFlight)
                .Publish(context => CreateCancelOutboundFlightCommand(context.Saga, context.Message.Reason))
        );

        During(CompensatingOutboundFlight,
            When(FlightCancelled)
                .Then(context => context.Saga.IsOutboundFlightReserved = false)
                .TransitionTo(ReleasingPayment)
                .Publish(context => CreateReleasePaymentCommand(context.Saga, context.Message.Reason))
        );

        During(ReleasingPayment,
            When(PaymentReleased)
                .Then(context => context.Saga.PaymentTransactionId = context.Message.PaymentAuthorisationId)
                .IfElse(context => context.Saga.IsCancelledByUser,
                    cancelled => cancelled
                        .TransitionTo(Cancelled)
                        .Publish(context => CreateTripBookingCancelledEvent(context.Saga, context.Message.Reason)),
                    notCancelled => notCancelled
                        .TransitionTo(Failed)
                        .Publish(context => CreateTripBookingFailedEvent(context.Saga, context.Message.Reason))
                )
        );
    }

    private void ConfigureTerminalStates()
    {
        // Completed state - handle refund requests and ignore redundant events
        During(Completed,
            When(TripRefundRequested)
                .Then(context => context.Saga.RefundReason = context.Message.Reason)
                .TransitionTo(AwaitingPaymentRefund)
                .Publish(context => CreateRefundPaymentCommand(context.Saga, context.Message.Reason)),
            Ignore(TripBookingCompleted),
            Ignore(TripBookingFailed),
            Ignore(FlightCancelled),
            Ignore(HotelCancelled),
            Ignore(GroundTransportCancelled),
            Ignore(InsuranceCancelled));

        // Failed state - actively cancel late resources to prevent orphaned data
        // These events were scheduled before the failure but arrived after
        During(Failed,
            Ignore(TripBookingFailed),

            // Active cancellation for late flight reservations
            When(OutboundFlightReserved)
                .Then(context => context.Saga.OutboundFlightId = context.Message.FlightReservationId)
                .Publish(context =>
                    CreateCancelOutboundFlightCommand(
                        context.Saga,
                        "Late outbound flight reservation received after SAGA failure - cancelling orphaned resource")
                ),
            When(ReturnFlightReserved)
                .Then(context => context.Saga.ReturnFlightId = context.Message.FlightReservationId)
                .Publish(context =>
                    CreateCancelReturnFlightCommand(
                        context.Saga,
                        "Late return flight reservation received after SAGA failure - cancelling orphaned resource")
                ),
            Ignore(FlightReservationFailed),
            Ignore(FlightCancelled),

            // Active cancellation for late hotel reservations
            When(HotelReserved)
                .Then(context => context.Saga.HotelReservationId = context.Message.HotelReservationId)
                .Publish(context =>
                    CreateCancelHotelCommand(
                        context.Saga,
                        "Late hotel reservation received after SAGA failure - cancelling orphaned resource")
                ),
            Ignore(HotelConfirmed),
            Ignore(HotelReservationFailed),
            Ignore(HotelConfirmationExpired),
            Ignore(HotelCancelled),

            // Active cancellation for late ground transport reservations
            When(GroundTransportReserved)
                .Then(context => context.Saga.GroundTransportId = context.Message.TransportReservationId)
                .Publish(context =>
                    CreateCancelGroundTransportCommand(
                        context.Saga,
                        "Late ground transport reservation received after SAGA failure - cancelling orphaned resource")),
            Ignore(GroundTransportReservationFailed),
            Ignore(GroundTransportCancelled),

            // Active cancellation for late insurance policies
            When(InsuranceIssued)
                .Then(context => context.Saga.InsurancePolicyId = context.Message.InsurancePolicyId)
                .Publish(context =>
                    CreateCancelInsuranceCommand(
                        context.Saga,
                        "Late insurance policy received after SAGA failure - cancelling orphaned resource")),
            Ignore(InsuranceIssueFailed),
            Ignore(InsuranceCancelled),

            // Ignore late payment events - already handled
            Ignore(PaymentCaptured),
            Ignore(PaymentCaptureFailed),
            Ignore(PaymentReleased));

        // TimedOut state - handle late payment authorisation
        During(TimedOut,
            When(PaymentAuthorised)
                .Then(context =>
                {
                    context.Saga.PaymentTransactionId = context.Message.PaymentAuthorisationId;
                    context.Saga.IsPaymentAuthorised = true;
                })
                .Publish(context =>
                    CreateReleasePaymentCommand(
                        context.Saga,
                        "Payment authorisation received after timeout - releasing funds")),
            When(PaymentReleased)
                .TransitionTo(Failed),
            Ignore(PaymentAuthorisationFailed),
            Ignore(TripBookingFailed));

        // Cancelled state - actively cancel late resources (same as Failed)
        During(Cancelled,
            Ignore(TripBookingCancelled),

            // Active cancellation for late flight reservations
            When(OutboundFlightReserved)
                .Then(context => context.Saga.OutboundFlightId = context.Message.FlightReservationId)
                .Publish(context =>
                    CreateCancelOutboundFlightCommand(
                        context.Saga,
                        "Late outbound flight reservation received after cancellation - cancelling orphaned resource")),
            When(ReturnFlightReserved)
                .Then(context => context.Saga.ReturnFlightId = context.Message.FlightReservationId)
                .Publish(context =>
                    CreateCancelReturnFlightCommand(
                        context.Saga,
                        "Late return flight reservation received after cancellation - cancelling orphaned resource")),
            Ignore(FlightReservationFailed),
            Ignore(FlightCancelled),

            // Active cancellation for late hotel reservations
            When(HotelReserved)
                .Then(context => context.Saga.HotelReservationId = context.Message.HotelReservationId)
                .Publish(context =>
                    CreateCancelHotelCommand(
                        context.Saga,
                        "Late hotel reservation received after cancellation - cancelling orphaned resource")),
            Ignore(HotelConfirmed),
            Ignore(HotelReservationFailed),
            Ignore(HotelCancelled),

            // Active cancellation for late ground transport
            When(GroundTransportReserved)
                .Then(context => context.Saga.GroundTransportId = context.Message.TransportReservationId)
                .Publish(context =>
                    CreateCancelGroundTransportCommand(
                        context.Saga,
                        "Late ground transport reservation received after cancellation - cancelling orphaned resource")),
            Ignore(GroundTransportReservationFailed),
            Ignore(GroundTransportCancelled),

            // Active cancellation for late insurance
            When(InsuranceIssued)
                .Then(context => context.Saga.InsurancePolicyId = context.Message.InsurancePolicyId)
                .Publish(context =>
                    CreateCancelInsuranceCommand(
                        context.Saga,
                        "Late insurance policy received after cancellation - cancelling orphaned resource")),
            Ignore(InsuranceIssueFailed),
            Ignore(InsuranceCancelled),

            // Handle late payment authorisation - release it
            When(PaymentAuthorised)
                .Then(context =>
                {
                    context.Saga.PaymentTransactionId = context.Message.PaymentAuthorisationId;
                    context.Saga.IsPaymentAuthorised = true;
                })
                .Publish(context =>
                    CreateReleasePaymentCommand(
                        context.Saga,
                        "Late payment authorisation received after cancellation - releasing funds")),
            Ignore(PaymentAuthorisationFailed),
            Ignore(PaymentReleased));

        // Refunded state - terminal state after successful refund
        During(Refunded,
            Ignore(TripBookingRefunded),
            Ignore(PaymentRefunded));
    }

    /// <summary>Helper methods for saga state initialization</summary>
    private static void InitializeSagaState(TripBookingSagaState saga, TripBookingCreated message)
    {
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
    }

    // Command factory methods
    private static AuthorisePayment CreateAuthorisePaymentCommand(TripBookingSagaState saga) =>
        new(CorrelationId: saga.CorrelationId,
            TripId: saga.TripId,
            CustomerId: saga.CustomerId,
            PaymentMethodId: saga.PaymentMethodId,
            Amount: saga.TotalAmount,
            Currency: saga.Currency);

    private static ReserveOutboundFlight CreateReserveOutboundFlightCommand(TripBookingSagaState saga) =>
        new(CorrelationId: saga.CorrelationId,
            TripId: saga.TripId,
            Origin: saga.Origin,
            Destination: saga.Destination,
            DepartureDate: saga.DepartureDate,
            FlightNumber: saga.OutboundFlightNumber,
            Carrier: saga.OutboundCarrier,
            PassengerCount: saga.NumberOfGuests);

    private static ReserveReturnFlight CreateReserveReturnFlightCommand(TripBookingSagaState saga) =>
        new(CorrelationId: saga.CorrelationId,
            TripId: saga.TripId,
            Origin: saga.Destination,
            Destination: saga.Origin,
            DepartureDate: saga.ReturnDate,
            FlightNumber: saga.ReturnFlightNumber,
            Carrier: saga.ReturnCarrier,
            PassengerCount: saga.NumberOfGuests);

    private static ReserveHotel CreateReserveHotelCommand(TripBookingSagaState saga) =>
        new(CorrelationId: saga.CorrelationId,
            TripId: saga.TripId,
            HotelId: saga.HotelId,
            HotelName: saga.HotelName,
            CheckIn: saga.CheckIn,
            CheckOut: saga.CheckOut,
            NumberOfGuests: saga.NumberOfGuests,
            GuestName: saga.CustomerName,
            GuestEmail: saga.CustomerEmail);

    private static ConfirmHotel CreateConfirmHotelCommand(TripBookingSagaState saga) =>
        new(CorrelationId: saga.CorrelationId,
            TripId: saga.TripId,
            HotelReservationId: saga.HotelReservationId!.Value);

    private static ReserveGroundTransport CreateReserveGroundTransportCommand(TripBookingSagaState saga) =>
        new(CorrelationId: saga.CorrelationId,
            TripId: saga.TripId,
            Type: saga.GroundTransportType!,
            PickupLocation: saga.GroundTransportPickupLocation!,
            DropoffLocation: saga.GroundTransportDropoffLocation!,
            PickupDate: saga.DepartureDate,
            PassengerCount: saga.NumberOfGuests);

    private static IssueInsurance CreateIssueInsuranceCommand(TripBookingSagaState saga) =>
        new(CorrelationId: saga.CorrelationId,
            TripId: saga.TripId,
            CustomerId: saga.CustomerId,
            CustomerName: saga.CustomerName,
            CustomerEmail: saga.CustomerEmail,
            OutboundFlightReservationId: saga.OutboundFlightId!.Value,
            ReturnFlightReservationId: saga.ReturnFlightId!.Value,
            HotelReservationId: saga.HotelReservationId!.Value,
            CoverageStartDate: saga.DepartureDate,
            CoverageEndDate: saga.ReturnDate,
            TripTotalValue: saga.TotalAmount);

    private static CapturePayment CreateCapturePaymentCommand(TripBookingSagaState saga) =>
        new(CorrelationId: saga.CorrelationId,
            TripId: saga.TripId,
            PaymentAuthorisationId: saga.PaymentTransactionId!.Value,
            Amount: saga.TotalAmount);

    private static ReleasePayment CreateReleasePaymentCommand(TripBookingSagaState saga, string reason) =>
        new(CorrelationId: saga.CorrelationId,
            TripId: saga.TripId,
            PaymentAuthorisationId: saga.PaymentTransactionId!.Value,
            Reason: reason);

    private static CancelFlight CreateCancelOutboundFlightCommand(TripBookingSagaState saga, string reason) =>
        new(CorrelationId: saga.CorrelationId,
            TripId: saga.TripId,
            FlightReservationId: saga.OutboundFlightId!.Value,
            Reason: reason);

    private static CancelFlight CreateCancelReturnFlightCommand(TripBookingSagaState saga, string reason) =>
        new(CorrelationId: saga.CorrelationId,
            TripId: saga.TripId,
            FlightReservationId: saga.ReturnFlightId!.Value,
            Reason: reason);

    private static CancelHotel CreateCancelHotelCommand(TripBookingSagaState saga, string reason) =>
        new(CorrelationId: saga.CorrelationId,
            TripId: saga.TripId,
            HotelReservationId: saga.HotelReservationId!.Value,
            Reason: reason);

    private static CancelGroundTransport CreateCancelGroundTransportCommand(TripBookingSagaState saga, string reason) =>
        new(CorrelationId: saga.CorrelationId,
            TripId: saga.TripId,
            TransportReservationId: saga.GroundTransportId!.Value,
            Reason: reason);

    private static CancelInsurance CreateCancelInsuranceCommand(TripBookingSagaState saga, string reason) =>
        new(CorrelationId: saga.CorrelationId,
            TripId: saga.TripId,
            InsurancePolicyId: saga.InsurancePolicyId!.Value,
            Reason: reason);

    private static TripBookingCompleted CreateTripBookingCompletedEvent(TripBookingSagaState saga) =>
        new(TripId: saga.TripId,
            CompletedAt: DateTime.UtcNow);

    private static TripBookingCancelled CreateTripBookingCancelledEvent(TripBookingSagaState saga, string reason) =>
        new(TripId: saga.TripId,
            Reason: reason,
            CancelledAt: DateTime.UtcNow);

    private static TripBookingFailed CreateTripBookingFailedEvent(TripBookingSagaState saga, string reason) =>
        new(TripId: saga.TripId,
            Reason: reason,
            FailedAt: DateTime.UtcNow);

    private static RefundPayment CreateRefundPaymentCommand(TripBookingSagaState saga, string reason) =>
        new(CorrelationId: saga.CorrelationId,
            TripId: saga.TripId,
            PaymentId: saga.PaymentTransactionId!.Value,
            Amount: saga.TotalAmount,
            Reason: reason);

    private static TripBookingRefunded CreateTripBookingRefundedEvent(TripBookingSagaState saga) =>
        new(TripId: saga.TripId,
            RefundedAmount: saga.TotalAmount,
            RefundedAt: DateTime.UtcNow);
}
