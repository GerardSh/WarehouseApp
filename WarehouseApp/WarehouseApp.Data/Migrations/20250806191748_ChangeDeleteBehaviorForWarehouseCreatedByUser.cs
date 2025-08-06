using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDeleteBehaviorForWarehouseCreatedByUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warehouses_AspNetUsers_CreatedByUserId",
                table: "Warehouses");

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_AspNetUsers_CreatedByUserId",
                table: "Warehouses",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warehouses_AspNetUsers_CreatedByUserId",
                table: "Warehouses");

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_AspNetUsers_CreatedByUserId",
                table: "Warehouses",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
