using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Notification.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_seed_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "Body", "Channel", "CreatedAt", "CustomerId", "ErrorMessage", "Recipient", "SentAt", "Status", "Subject", "TripId", "Type" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Dear John, your trip booking (ID: aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa) has been confirmed. Thank you for choosing our travel platform!", "Email", new DateTime(2025, 6, 15, 10, 30, 0, 0, DateTimeKind.Utc), new Guid("c1111111-1111-1111-1111-111111111111"), null, "john.doe@example.com", new DateTime(2025, 6, 15, 10, 30, 5, 0, DateTimeKind.Utc), "Sent", "Your Trip Booking is Confirmed!", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "BookingConfirmation" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Your trip booking aaaaaaaa has been confirmed. Check your email for details.", "Sms", new DateTime(2025, 6, 15, 10, 30, 10, 0, DateTimeKind.Utc), new Guid("c1111111-1111-1111-1111-111111111111"), null, "+1234567890", new DateTime(2025, 6, 15, 10, 30, 12, 0, DateTimeKind.Utc), "Sent", "Booking Confirmed", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "BookingConfirmation" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Dear Jane, we regret to inform you that your trip booking (ID: bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb) could not be completed due to payment authorization failure. Please try again or contact support.", "Email", new DateTime(2025, 7, 20, 14, 45, 0, 0, DateTimeKind.Utc), new Guid("c2222222-2222-2222-2222-222222222222"), null, "jane.smith@example.com", new DateTime(2025, 7, 20, 14, 45, 3, 0, DateTimeKind.Utc), "Sent", "Trip Booking Failed - Action Required", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "BookingFailure" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Dear Bob, your trip booking (ID: cccccccc-cccc-cccc-cccc-cccccccccccc) has been cancelled as per your request. A refund will be processed within 5-7 business days.", "Email", new DateTime(2025, 8, 5, 9, 15, 0, 0, DateTimeKind.Utc), new Guid("c3333333-3333-3333-3333-333333333333"), null, "bob.wilson@example.com", new DateTime(2025, 8, 5, 9, 15, 2, 0, DateTimeKind.Utc), "Sent", "Your Trip Booking Has Been Cancelled", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Cancellation" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Your trip cccccccc has been cancelled. Refund processing started.", "Push", new DateTime(2025, 8, 5, 9, 15, 5, 0, DateTimeKind.Utc), new Guid("c3333333-3333-3333-3333-333333333333"), "Push notification service unavailable", "device-token-xyz789", null, "Failed", "Booking Cancelled", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Cancellation" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));
        }
    }
}
