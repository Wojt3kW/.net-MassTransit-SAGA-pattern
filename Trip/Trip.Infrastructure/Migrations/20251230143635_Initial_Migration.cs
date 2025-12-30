using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TripBookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OutboundFlightConfirmation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReturnFlightConfirmation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HotelConfirmation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GroundTransportConfirmation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InsurancePolicyNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentConfirmation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripBookings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TripBookings_CustomerId",
                table: "TripBookings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TripBookings_Status",
                table: "TripBookings",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TripBookings");
        }
    }
}
