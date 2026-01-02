using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripBooking.Saga.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TripBookingSagaStates",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentState = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    OutboundFlightNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OutboundCarrier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReturnFlightNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReturnCarrier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HotelId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    HotelName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CheckIn = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    CheckOut = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    NumberOfGuests = table.Column<int>(type: "int", nullable: false),
                    IncludeGroundTransport = table.Column<bool>(type: "bit", nullable: false),
                    GroundTransportType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GroundTransportPickupLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GroundTransportDropoffLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IncludeInsurance = table.Column<bool>(type: "bit", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PaymentTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OutboundFlightId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReturnFlightId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HotelReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GroundTransportId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InsurancePolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsPaymentAuthorised = table.Column<bool>(type: "bit", nullable: false),
                    IsOutboundFlightReserved = table.Column<bool>(type: "bit", nullable: false),
                    IsReturnFlightReserved = table.Column<bool>(type: "bit", nullable: false),
                    IsHotelReserved = table.Column<bool>(type: "bit", nullable: false),
                    IsHotelConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    IsGroundTransportReserved = table.Column<bool>(type: "bit", nullable: false),
                    IsInsuranceIssued = table.Column<bool>(type: "bit", nullable: false),
                    IsPaymentCaptured = table.Column<bool>(type: "bit", nullable: false),
                    PaymentAuthorisationTimeoutToken = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OutboundFlightTimeoutToken = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReturnFlightTimeoutToken = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HotelReservationTimeoutToken = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HotelConfirmationTimeoutToken = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GroundTransportTimeoutToken = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InsuranceTimeoutToken = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaymentCaptureTimeoutToken = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserInactivityTimeoutToken = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaymentCaptureRetryCount = table.Column<int>(type: "int", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripBookingSagaStates", x => x.CorrelationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TripBookingSagaStates_CurrentState",
                table: "TripBookingSagaStates",
                column: "CurrentState");

            migrationBuilder.CreateIndex(
                name: "IX_TripBookingSagaStates_CustomerId",
                table: "TripBookingSagaStates",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TripBookingSagaStates_TripId",
                table: "TripBookingSagaStates",
                column: "TripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TripBookingSagaStates");
        }
    }
}
