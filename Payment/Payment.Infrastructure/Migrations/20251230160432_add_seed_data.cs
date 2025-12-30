using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Payment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_seed_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PaymentTransactions",
                columns: new[] { "Id", "Amount", "AuthorisationCode", "AuthorisedAt", "CapturedAt", "CardHolderName", "CardLastFourDigits", "CreatedAt", "Currency", "CustomerId", "FailureReason", "RefundedAt", "ReleasedAt", "Status", "TripId" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 1250.00m, "AUTH-001-XYZ", new DateTime(2025, 1, 15, 10, 0, 5, 0, DateTimeKind.Utc), null, "John Smith", "4242", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "GBP", new Guid("c1111111-1111-1111-1111-111111111111"), null, null, null, "Authorised", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 2499.99m, "AUTH-002-ABC", new DateTime(2025, 1, 10, 14, 30, 3, 0, DateTimeKind.Utc), new DateTime(2025, 1, 10, 15, 45, 0, 0, DateTimeKind.Utc), "Jane Doe", "1234", new DateTime(2025, 1, 10, 14, 30, 0, 0, DateTimeKind.Utc), "USD", new Guid("c2222222-2222-2222-2222-222222222222"), null, null, null, "Captured", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("33333333-3333-3333-3333-333333333333"), 899.50m, "AUTH-003-DEF", new DateTime(2025, 1, 5, 9, 0, 2, 0, DateTimeKind.Utc), new DateTime(2025, 1, 5, 10, 0, 0, 0, DateTimeKind.Utc), "Bob Wilson", "5678", new DateTime(2025, 1, 5, 9, 0, 0, 0, DateTimeKind.Utc), "EUR", new Guid("c3333333-3333-3333-3333-333333333333"), null, new DateTime(2025, 1, 8, 16, 30, 0, 0, DateTimeKind.Utc), null, "Refunded", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc") },
                    { new Guid("44444444-4444-4444-4444-444444444444"), 150.00m, "AUTH-004-GHI", new DateTime(2025, 1, 12, 11, 0, 4, 0, DateTimeKind.Utc), null, "John Smith", "9999", new DateTime(2025, 1, 12, 11, 0, 0, 0, DateTimeKind.Utc), "GBP", new Guid("c1111111-1111-1111-1111-111111111111"), null, null, new DateTime(2025, 1, 12, 12, 0, 0, 0, DateTimeKind.Utc), "Released", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PaymentTransactions",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "PaymentTransactions",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "PaymentTransactions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "PaymentTransactions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));
        }
    }
}
