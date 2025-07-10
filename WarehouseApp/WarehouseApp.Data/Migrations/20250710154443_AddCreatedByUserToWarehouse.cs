using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByUserToWarehouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Warehouses",
                type: "uniqueidentifier",
                nullable: true,
                comment: "ID of the user who created the warehouse");

            migrationBuilder.UpdateData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("a2b3c4d5-6e78-4f9a-bc12-d4e5f6a7b891"),
                column: "CreatedByUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("b4c5d6e7-8f91-42a3-bc12-e3f4a5d6b782"),
                column: "CreatedByUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470"),
                column: "CreatedByUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("be8f00a5-682d-4b43-9734-d3e17078cb52"),
                column: "CreatedByUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("c3e1a7d3-8e44-4f9b-bf8b-1d3f6e7f8d42"),
                column: "CreatedByUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("c6d7e8f9-1a23-45b4-bc12-d3e4f5a6b891"),
                column: "CreatedByUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("d5f6b4e8-3b67-4a7d-bc12-f4a9e6c8d351"),
                column: "CreatedByUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("d8e9f1a2-3b45-47c6-bc12-e2f3a4d5b691"),
                column: "CreatedByUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("e8a9c5b7-4d23-49fb-a91b-c6e1f2d8b643"),
                column: "CreatedByUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("f1c2d4a6-5b34-42d8-9c12-e3a7b8f6d921"),
                column: "CreatedByUserId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_CreatedByUserId",
                table: "Warehouses",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_Name_CreatedByUserId",
                table: "Warehouses",
                columns: new[] { "Name", "CreatedByUserId" },
                unique: true,
                filter: "[CreatedByUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_AspNetUsers_CreatedByUserId",
                table: "Warehouses",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warehouses_AspNetUsers_CreatedByUserId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_CreatedByUserId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_Name_CreatedByUserId",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Warehouses");
        }
    }
}
