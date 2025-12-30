using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlightReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FlightNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Carrier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    ConfirmationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PassengerCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightReservations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightReservations_FlightNumber",
                table: "FlightReservations",
                column: "FlightNumber");

            migrationBuilder.CreateIndex(
                name: "IX_FlightReservations_Status",
                table: "FlightReservations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FlightReservations_TripId",
                table: "FlightReservations",
                column: "TripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightReservations");
        }
    }
}
