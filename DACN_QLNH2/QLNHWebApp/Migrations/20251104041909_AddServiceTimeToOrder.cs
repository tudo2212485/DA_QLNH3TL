using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNHWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceTimeToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ServiceEndTime",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ServiceStartTime",
                table: "Orders",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceEndTime",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ServiceStartTime",
                table: "Orders");
        }
    }
}
