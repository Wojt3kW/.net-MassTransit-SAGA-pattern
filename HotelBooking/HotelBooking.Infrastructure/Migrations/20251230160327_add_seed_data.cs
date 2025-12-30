using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_seed_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "HotelReservations",
                columns: new[] { "Id", "CancellationReason", "CancelledAt", "CheckIn", "CheckOut", "ConfirmationCode", "ConfirmedAt", "CreatedAt", "ExpiresAt", "GuestEmail", "GuestName", "HotelId", "HotelName", "NumberOfGuests", "PricePerNight", "Status", "TotalPrice", "TripId" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), null, null, new DateTime(2025, 3, 15, 15, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 3, 22, 11, 0, 0, 0, DateTimeKind.Utc), "HT-PLAZA001", new DateTime(2025, 1, 10, 14, 20, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 10, 14, 5, 0, 0, DateTimeKind.Utc), null, "john.smith@email.com", "John Smith", "HTL-NYC-001", "The Plaza Hotel", 2, 450.00m, "Confirmed", 3150.00m, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), null, null, new DateTime(2025, 4, 1, 15, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 4, 7, 11, 0, 0, 0, DateTimeKind.Utc), "HT-MBS002", new DateTime(2025, 1, 15, 9, 50, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 15, 9, 35, 0, 0, DateTimeKind.Utc), null, "anna.mueller@email.de", "Anna Mueller", "HTL-SIN-001", "Marina Bay Sands", 1, 580.00m, "Confirmed", 3480.00m, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Trip cancelled by customer", new DateTime(2025, 1, 21, 10, 5, 0, 0, DateTimeKind.Utc), new DateTime(2025, 5, 10, 15, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 5, 15, 11, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTime(2025, 1, 20, 16, 50, 0, 0, DateTimeKind.Utc), null, "pierre.dubois@email.fr", "Pierre Dubois", "HTL-LAX-001", "Beverly Wilshire", 3, 620.00m, "Cancelled", 3100.00m, new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc") },
                    { new Guid("44444444-4444-4444-4444-444444444444"), null, null, new DateTime(2025, 6, 1, 15, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 5, 11, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTime(2025, 1, 25, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 25, 11, 15, 0, 0, DateTimeKind.Utc), "maria.garcia@email.es", "Maria Garcia", "HTL-LON-001", "The Savoy", 2, 520.00m, "Reserved", 2080.00m, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "HotelReservations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "HotelReservations",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "HotelReservations",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "HotelReservations",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));
        }
    }
}
