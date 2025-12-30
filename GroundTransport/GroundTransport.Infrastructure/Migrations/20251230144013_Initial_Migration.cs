using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroundTransport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransportReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PickupLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DropoffLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PickupDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    PassengerCount = table.Column<int>(type: "int", nullable: false),
                    ConfirmationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportReservations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransportReservations_Status",
                table: "TransportReservations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TransportReservations_TripId",
                table: "TransportReservations",
                column: "TripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransportReservations");
        }
    }
}
