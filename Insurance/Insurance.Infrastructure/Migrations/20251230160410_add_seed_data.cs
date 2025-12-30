using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Insurance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_seed_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "InsurancePolicies",
                columns: new[] { "Id", "CancellationReason", "CancelledAt", "CoverageEndDate", "CoverageStartDate", "CreatedAt", "CustomerEmail", "CustomerId", "CustomerName", "HotelReservationId", "IssuedAt", "OutboundFlightReservationId", "PolicyNumber", "Premium", "ReturnFlightReservationId", "Status", "TripId", "TripTotalValue" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), null, null, new DateTime(2025, 3, 22, 23, 59, 59, 0, DateTimeKind.Utc), new DateTime(2025, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 10, 14, 0, 0, 0, DateTimeKind.Utc), "john.smith@example.com", new Guid("c1111111-1111-1111-1111-111111111111"), "John Smith", new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 10, 14, 1, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), "INS-2025-001", 125.00m, new Guid("22222222-2222-2222-2222-222222222222"), "Issued", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 2500.00m },
                    { new Guid("22222222-2222-2222-2222-222222222222"), null, null, new DateTime(2025, 4, 10, 23, 59, 59, 0, DateTimeKind.Utc), new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 15, 9, 30, 0, 0, DateTimeKind.Utc), "jane.doe@example.com", new Guid("c2222222-2222-2222-2222-222222222222"), "Jane Doe", new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 15, 9, 31, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), "INS-2025-002", 160.00m, new Guid("44444444-4444-4444-4444-444444444444"), "Issued", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 3200.00m },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Trip cancelled by customer", new DateTime(2025, 1, 25, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 5, 27, 23, 59, 59, 0, DateTimeKind.Utc), new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 20, 16, 0, 0, 0, DateTimeKind.Utc), "robert.johnson@example.com", new Guid("c3333333-3333-3333-3333-333333333333"), "Robert Johnson", new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2025, 1, 20, 16, 1, 0, 0, DateTimeKind.Utc), new Guid("55555555-5555-5555-5555-555555555555"), "INS-2025-003", 90.00m, new Guid("66666666-6666-6666-6666-666666666666"), "Cancelled", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), 1800.00m },
                    { new Guid("44444444-4444-4444-4444-444444444444"), null, null, new DateTime(2025, 6, 17, 23, 59, 59, 0, DateTimeKind.Utc), new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 28, 11, 0, 0, 0, DateTimeKind.Utc), "john.smith@example.com", new Guid("c1111111-1111-1111-1111-111111111111"), "John Smith", new Guid("44444444-4444-4444-4444-444444444444"), null, new Guid("77777777-7777-7777-7777-777777777777"), null, 105.00m, new Guid("88888888-8888-8888-8888-888888888888"), "Pending", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 2100.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "InsurancePolicies",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "InsurancePolicies",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "InsurancePolicies",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "InsurancePolicies",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));
        }
    }
}
