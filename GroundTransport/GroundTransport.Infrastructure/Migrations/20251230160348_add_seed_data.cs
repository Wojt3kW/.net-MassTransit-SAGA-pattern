using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GroundTransport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_seed_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TransportReservations",
                columns: new[] { "Id", "CancellationReason", "CancelledAt", "ConfirmationCode", "ConfirmedAt", "CreatedAt", "DropoffLocation", "PassengerCount", "PickupDate", "PickupLocation", "Price", "Status", "TripId", "Type" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555555"), null, null, "TR-JFK001", new DateTime(2025, 1, 10, 14, 5, 5, 0, DateTimeKind.Utc), new DateTime(2025, 1, 10, 14, 5, 0, 0, DateTimeKind.Utc), "The Plaza Hotel, 768 5th Ave, New York", 2, new DateTime(2025, 3, 15, 14, 30, 0, 0, DateTimeKind.Utc), "JFK International Airport, Terminal 7", 85.00m, "Confirmed", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "AirportTransfer" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), null, null, "TR-JFK002", new DateTime(2025, 1, 10, 14, 5, 15, 0, DateTimeKind.Utc), new DateTime(2025, 1, 10, 14, 5, 10, 0, DateTimeKind.Utc), "JFK International Airport, Terminal 7", 2, new DateTime(2025, 3, 22, 15, 0, 0, 0, DateTimeKind.Utc), "The Plaza Hotel, 768 5th Ave, New York", 85.00m, "Confirmed", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "AirportTransfer" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), null, null, "TR-SIN001", new DateTime(2025, 1, 15, 9, 35, 5, 0, DateTimeKind.Utc), new DateTime(2025, 1, 15, 9, 35, 0, 0, DateTimeKind.Utc), "Singapore Changi Airport, Terminal 3", 1, new DateTime(2025, 4, 2, 8, 0, 0, 0, DateTimeKind.Utc), "Singapore Changi Airport, Terminal 3", 320.00m, "Confirmed", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "CarRental" },
                    { new Guid("88888888-8888-8888-8888-888888888888"), null, null, null, null, new DateTime(2025, 2, 1, 16, 45, 0, 0, DateTimeKind.Utc), "Beverly Hills Hotel, 9641 Sunset Blvd", 3, new DateTime(2025, 5, 10, 15, 0, 0, 0, DateTimeKind.Utc), "LAX International Airport, Terminal B", 95.00m, "Pending", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "AirportTransfer" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TransportReservations",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "TransportReservations",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "TransportReservations",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "TransportReservations",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"));
        }
    }
}
