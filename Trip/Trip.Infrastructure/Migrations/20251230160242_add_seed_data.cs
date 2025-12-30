using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Trip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_seed_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TripBookings",
                columns: new[] { "Id", "CancelledAt", "CompletedAt", "CreatedAt", "CustomerEmail", "CustomerId", "FailureReason", "GroundTransportConfirmation", "HotelConfirmation", "InsurancePolicyNumber", "OutboundFlightConfirmation", "PaymentConfirmation", "ReturnFlightConfirmation", "Status", "TotalAmount" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null, new DateTime(2025, 1, 10, 14, 25, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 10, 14, 0, 0, 0, DateTimeKind.Utc), "john.smith@email.com", new Guid("c1111111-1111-1111-1111-111111111111"), null, "GT-NYC001", "HT-PLAZA001", "INS-2025-001", "FL-BA256001", "PAY-AUTH-001", "FL-BA257002", "Completed", 5770.00m },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), null, new DateTime(2025, 1, 15, 9, 55, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 15, 9, 30, 0, 0, DateTimeKind.Utc), "anna.mueller@email.de", new Guid("c2222222-2222-2222-2222-222222222222"), null, "GT-SIN001", "HT-MBS002", "INS-2025-002", "FL-LH440003", "PAY-CAP-002", null, "Completed", 4730.00m },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new DateTime(2025, 1, 21, 10, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2025, 1, 20, 16, 45, 0, 0, DateTimeKind.Utc), "pierre.dubois@email.fr", new Guid("c3333333-3333-3333-3333-333333333333"), "Customer requested cancellation", null, "HT-BW003", null, "FL-AF123004", null, null, "Cancelled", 3880.00m },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), null, null, new DateTime(2025, 1, 25, 11, 0, 0, 0, DateTimeKind.Utc), "maria.garcia@email.es", new Guid("c4444444-4444-4444-4444-444444444444"), null, null, null, null, null, null, null, "Processing", 0m },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), null, null, new DateTime(2025, 1, 28, 8, 0, 0, 0, DateTimeKind.Utc), "test.user@email.com", new Guid("c5555555-5555-5555-5555-555555555555"), null, null, null, null, null, null, null, "Pending", 0m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TripBookings",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "TripBookings",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "TripBookings",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "TripBookings",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));

            migrationBuilder.DeleteData(
                table: "TripBookings",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"));
        }
    }
}
