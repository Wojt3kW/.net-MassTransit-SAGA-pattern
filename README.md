# Travel Booking Platform - SAGA Pattern Learning Project

> 🎯 **Learning project** for mastering the Orchestration-based SAGA pattern using MassTransit in .NET 10.

## Domain

Enterprise-grade Travel Booking Platform that orchestrates:
- ✈️ Round-trip flights (outbound + return)
- 🏨 Hotel accommodation
- 🚗 Ground transportation
- 🛡️ Travel insurance
- 💳 Payment processing (authorise → capture → release)

## Technology Stack

| Category | Technology |
|----------|------------|
| Framework | .NET 10, C# 14 |
| Database | MS SQL Server (per microservice) |
| Messaging | RabbitMQ + MassTransit |
| CQRS | MediatR |
| Architecture | **Clean Architecture**, Vertical Slice, REPR Pattern |
| Orchestration | .NET Aspire |
| API | Minimal API + OpenAPI |
| Health Checks | SQL Server, RabbitMQ, MassTransit |

## Solution Structure

```
SAGA pattern.sln
├── SAGA pattern.AppHost/          # Aspire orchestrator
├── SAGA pattern.ServiceDefaults/  # Shared service configuration
│   ├── Extensions.cs
│   └── Settings/
│       ├── ApiSettings.cs
│       └── ConnectionStrings.cs
│
├── Trip/
│   ├── Trip.Domain/               # Entities (zero dependencies)
│   ├── Trip.Application/          # Repository interfaces
│   ├── Trip.Infrastructure/       # DbContext, Repository implementations
│   ├── Trip.API/                  # Entry point, REST API, starts SAGA
│   │   └── Consumers/             # Event consumers (updates TripBookings)
│   └── Trip.Contracts/            # Commands, Events, DTOs
│
├── TripBooking.Saga/
│   ├── TripBooking.Saga/          # SAGA State Machine library
│   │   ├── StateMachines/
│   │   ├── States/
│   │   └── Persistence/
│   └── TripBooking.Saga.API/      # SAGA host & monitoring endpoints
│
├── FlightBooking/
│   ├── FlightBooking.Domain/
│   ├── FlightBooking.Application/
│   ├── FlightBooking.Infrastructure/
│   ├── FlightBooking.API/         # MassTransit consumers for flights
│   └── FlightBooking.Contracts/
│
├── HotelBooking/
│   ├── HotelBooking.Domain/
│   ├── HotelBooking.Application/
│   ├── HotelBooking.Infrastructure/
│   ├── HotelBooking.API/          # MassTransit consumers for hotels
│   └── HotelBooking.Contracts/
│
├── GroundTransport/
│   ├── GroundTransport.Domain/
│   ├── GroundTransport.Application/
│   ├── GroundTransport.Infrastructure/
│   ├── GroundTransport.API/       # MassTransit consumers for transport
│   └── GroundTransport.Contracts/
│
├── Insurance/
│   ├── Insurance.Domain/
│   ├── Insurance.Application/
│   ├── Insurance.Infrastructure/
│   ├── Insurance.API/             # MassTransit consumers for insurance
│   └── Insurance.Contracts/
│
├── Payment/
│   ├── Payment.Domain/
│   ├── Payment.Application/
│   ├── Payment.Infrastructure/
│   ├── Payment.API/               # MassTransit consumers for payments
│   └── Payment.Contracts/
│
└── Notification/
    ├── Notification.Domain/
    ├── Notification.Application/
    ├── Notification.Infrastructure/
    ├── Notification.API/          # MassTransit consumers for notifications
    └── Notification.Contracts/
```

## Clean Architecture Layers

```
┌─────────────────────────────────────────────┐
│                   API                       │  ← Presentation (Endpoints, Consumers)
├─────────────────────────────────────────────┤
│              Infrastructure                 │  ← Persistence (DbContext, Repositories)
├─────────────────────────────────────────────┤
│               Application                   │  ← Use Cases (Repository Interfaces)
├─────────────────────────────────────────────┤
│                 Domain                      │  ← Entities (Zero Dependencies)
└─────────────────────────────────────────────┘

Dependency Direction: Domain ← Application ← Infrastructure ← API
```

## Microservices Overview

| Service | Role | Database | Commands |
|---------|------|----------|----------|
| **Trip.API** | REST entry point, starts SAGA | TripDb | CreateTrip, GetTrip, CancelTrip |
| **TripBooking.Saga.API** | SAGA monitoring & management | TripBookingSagaDb | GetSagaState, ListSagas |
| **FlightBooking.API** | Flight reservations | FlightBookingDb | ReserveOutboundFlight, ReserveReturnFlight, CancelFlight |
| **HotelBooking.API** | Hotel reservations | HotelBookingDb | ReserveHotel, ConfirmHotel, CancelHotel |
| **GroundTransport.API** | Ground transport | GroundTransportDb | ReserveGroundTransport, CancelGroundTransport |
| **Insurance.API** | Travel insurance | InsuranceDb | IssueInsurance, CancelInsurance |
| **Payment.API** | Payment processing | PaymentDb | AuthorisePayment, CapturePayment, ReleasePayment, RefundPayment |
| **Notification.API** | Customer notifications | NotificationDb | SendBookingConfirmation, SendBookingFailure, SendCancellation |

## Getting Started

### Prerequisites
- .NET 10 SDK
- Docker Desktop
- .NET Aspire workload

### Run with Aspire

```powershell
dotnet run --project "SAGA pattern.AppHost"
```

## Learning Goals

1. ✅ Understand microservices architecture
2. ✅ Set up .NET Aspire orchestration
3. ✅ Implement Clean Architecture layers
4. ✅ **Implement SAGA State Machine** - Happy path completed!
5. ✅ **Handle compensations** - All failure scenarios with retry!
6. ✅ **Failure simulation** - Test triggers in all consumers!
7. ✅ **Implement timeouts** - Quartz scheduler with all 8 timeouts!
8. 🎯 Implement User Cancellation flow
9. 🎯 Implement Inbox/Outbox patterns

## Testing

See `Trip/Trip.API/Trip.API.http` for all test requests including failure simulation.

See `SAGA-FLOWS.md` for detailed failure triggers and compensation flows.

## References

- [MassTransit State Machine Saga](https://masstransit.io/documentation/patterns/saga/state-machine)
- [SAGA Pattern](https://microservices.io/patterns/data/saga.html)
- [Vertical Slice Architecture](https://www.jimmybogard.com/vertical-slice-architecture/)