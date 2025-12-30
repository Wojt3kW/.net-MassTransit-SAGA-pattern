using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlightBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_seed_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FlightReservations",
                columns: new[] { "Id", "CancellationReason", "CancelledAt", "Carrier", "ConfirmationCode", "ConfirmedAt", "CreatedAt", "DepartureDate", "Destination", "FlightNumber", "Origin", "PassengerCount", "Price", "Status", "TripId", "Type" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), null, null, "British Airways", "FL-BA256001", new DateTime(2025, 1, 10, 14, 0, 5, 0, DateTimeKind.Utc), new DateTime(2025, 1, 10, 14, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 3, 15, 10, 30, 0, 0, DateTimeKind.Utc), "JFK", "BA256", "LHR", 2, 850.00m, "Confirmed", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Outbound" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), null, null, "British Airways", "FL-BA257002", new DateTime(2025, 1, 10, 14, 0, 15, 0, DateTimeKind.Utc), new DateTime(2025, 1, 10, 14, 0, 10, 0, DateTimeKind.Utc), new DateTime(2025, 3, 22, 18, 45, 0, 0, DateTimeKind.Utc), "LHR", "BA257", "JFK", 2, 920.00m, "Confirmed", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Return" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), null, null, "Lufthansa", "FL-LH440003", new DateTime(2025, 1, 15, 9, 30, 5, 0, DateTimeKind.Utc), new DateTime(2025, 1, 15, 9, 30, 0, 0, DateTimeKind.Utc), new DateTime(2025, 4, 1, 22, 15, 0, 0, DateTimeKind.Utc), "SIN", "LH440", "FRA", 1, 1250.00m, "Confirmed", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Outbound" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Customer requested cancellation", new DateTime(2025, 1, 21, 10, 0, 0, 0, DateTimeKind.Utc), "Air France", null, null, new DateTime(2025, 1, 20, 16, 45, 0, 0, DateTimeKind.Utc), new DateTime(2025, 5, 10, 11, 0, 0, 0, DateTimeKind.Utc), "LAX", "AF123", "CDG", 3, 780.00m, "Cancelled", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Outbound" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FlightReservations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "FlightReservations",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "FlightReservations",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "FlightReservations",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));
        }
    }
}
