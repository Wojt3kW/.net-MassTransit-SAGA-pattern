using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripBooking.Saga.Migrations
{
    /// <inheritdoc />
    public partial class Alter_TripBookingSagaState_Add_IsRefunded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRefunded",
                table: "TripBookingSagaStates",
                type: "bit",
                nullable: true);

            migrationBuilder.Sql(@"UPDATE TripBookingSagaStates SET IsRefunded = 0");

            migrationBuilder.AddColumn<string>(
                name: "RefundReason",
                table: "TripBookingSagaStates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRefunded",
                table: "TripBookingSagaStates");

            migrationBuilder.DropColumn(
                name: "RefundReason",
                table: "TripBookingSagaStates");
        }
    }
}
