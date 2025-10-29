using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNHWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTableBookingIdToOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_TableBookings_TableBookingId",
                table: "OrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "OrderItems",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_TableBookings_TableBookingId",
                table: "OrderItems",
                column: "TableBookingId",
                principalTable: "TableBookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_TableBookings_TableBookingId",
                table: "OrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_TableBookings_TableBookingId",
                table: "OrderItems",
                column: "TableBookingId",
                principalTable: "TableBookings",
                principalColumn: "Id");
        }
    }
}
