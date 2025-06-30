using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationUserWarehouseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "AspNetUsers",
                comment: "Extended Identity user with application-specific properties");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Full name of the user",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "UsersWarehouses",
                columns: table => new
                {
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to the ApplicationUser"),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersWarehouses", x => new { x.ApplicationUserId, x.WarehouseId });
                    table.ForeignKey(
                        name: "FK_UsersWarehouses_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersWarehouses_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Mapping table between application users and warehouses");

            migrationBuilder.CreateIndex(
                name: "IX_UsersWarehouses_WarehouseId",
                table: "UsersWarehouses",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersWarehouses");

            migrationBuilder.AlterTable(
                name: "AspNetUsers",
                oldComment: "Extended Identity user with application-specific properties");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Full name of the user");
        }
    }
}
