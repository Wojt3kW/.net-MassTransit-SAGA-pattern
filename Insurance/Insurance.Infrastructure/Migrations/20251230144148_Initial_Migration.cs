using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insurance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InsurancePolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PolicyNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CoverageStartDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    CoverageEndDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    OutboundFlightReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReturnFlightReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TripTotalValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Premium = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsurancePolicies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePolicies_CustomerId",
                table: "InsurancePolicies",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePolicies_PolicyNumber",
                table: "InsurancePolicies",
                column: "PolicyNumber");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePolicies_Status",
                table: "InsurancePolicies",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePolicies_TripId",
                table: "InsurancePolicies",
                column: "TripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InsurancePolicies");
        }
    }
}
