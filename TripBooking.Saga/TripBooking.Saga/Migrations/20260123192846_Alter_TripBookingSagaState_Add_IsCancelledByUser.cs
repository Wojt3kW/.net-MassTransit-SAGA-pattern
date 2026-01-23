using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripBooking.Saga.Migrations
{
    /// <inheritdoc />
    public partial class Alter_TripBookingSagaState_Add_IsCancelledByUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCancelledByUser",
                table: "TripBookingSagaStates",
                type: "bit",
                nullable: true);

            migrationBuilder.Sql(@"UPDATE TripBookingSagaStates SET IsCancelledByUser = 0");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCancelledByUser",
                table: "TripBookingSagaStates",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelledByUser",
                table: "TripBookingSagaStates");
        }
    }
}
