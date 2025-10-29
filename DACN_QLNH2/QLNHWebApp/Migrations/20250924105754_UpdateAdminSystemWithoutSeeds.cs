using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QLNHWebApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminSystemWithoutSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RestaurantSettings",
                keyColumn: "Id",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CreatedAt", "FullName", "IsActive", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 24, 17, 52, 33, 77, DateTimeKind.Local).AddTicks(4173), "Admin Hệ Thống", true, "$2a$11$bby4vLvwdWQlC1Whe.7jnON.iDW5H2AYVCWLw6kc6xcy7vapK/Xn.", "Admin", "admin" },
                    { 2, new DateTime(2025, 9, 24, 17, 52, 33, 311, DateTimeKind.Local).AddTicks(1430), "Nguyễn Văn A", true, "$2a$11$BFH6ubEc/XUi4fkEWrc3CuUS3EKt2qJfcqR8.Tj8DEo7qGZWdz9Oy", "Waiter", "waiter1" },
                    { 3, new DateTime(2025, 9, 24, 17, 52, 33, 423, DateTimeKind.Local).AddTicks(8739), "Trần Thị B", true, "$2a$11$ni6oZuvnNWQ7iVGTR9KMXe3jaLlFcPSDuFKbYPsj2842FjLDOsbne", "Chef", "chef1" }
                });

            migrationBuilder.InsertData(
                table: "RestaurantSettings",
                columns: new[] { "Id", "Address", "CloseTime", "Email", "OpenTime", "Phone", "RestaurantName", "TaxRate" },
                values: new object[] { 1, "123 Đường ABC, Quận 1, TP.HCM", new TimeOnly(22, 0, 0), "info@3tlrestaurant.com", new TimeOnly(10, 0, 0), "028.1234.5678", "Nhà Hàng 3TL", 0.1m });
        }
    }
}
