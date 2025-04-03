using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WarehouseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedWarehouses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "Id", "Address", "IsDeleted", "Name", "Size" },
                values: new object[,]
                {
                    { new Guid("a2b3c4d5-6e78-4f9a-bc12-d4e5f6a7b891"), "Location G", false, "Warehouse G", 5900.0 },
                    { new Guid("b4c5d6e7-8f91-42a3-bc12-e3f4a5d6b782"), "Location H", false, "Warehouse H", 4800.0 },
                    { new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470"), "Location A", false, "Warehouse A", 4650.0 },
                    { new Guid("be8f00a5-682d-4b43-9734-d3e17078cb52"), "Location B", false, "Warehouse B", 5200.0 },
                    { new Guid("c3e1a7d3-8e44-4f9b-bf8b-1d3f6e7f8d42"), "Location C", false, "Warehouse C", 4500.0 },
                    { new Guid("c6d7e8f9-1a23-45b4-bc12-d3e4f5a6b891"), "Location I", false, "Warehouse I", 5200.0 },
                    { new Guid("d5f6b4e8-3b67-4a7d-bc12-f4a9e6c8d351"), "Location D", false, "Warehouse D", 6300.0 },
                    { new Guid("d8e9f1a2-3b45-47c6-bc12-e2f3a4d5b691"), "Location J", false, "Warehouse J", 6000.0 },
                    { new Guid("e8a9c5b7-4d23-49fb-a91b-c6e1f2d8b643"), "Location E", false, "Warehouse E", 7100.0 },
                    { new Guid("f1c2d4a6-5b34-42d8-9c12-e3a7b8f6d921"), "Location F", false, "Warehouse F", 5400.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("a2b3c4d5-6e78-4f9a-bc12-d4e5f6a7b891"));

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("b4c5d6e7-8f91-42a3-bc12-e3f4a5d6b782"));

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470"));

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("be8f00a5-682d-4b43-9734-d3e17078cb52"));

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("c3e1a7d3-8e44-4f9b-bf8b-1d3f6e7f8d42"));

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("c6d7e8f9-1a23-45b4-bc12-d3e4f5a6b891"));

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("d5f6b4e8-3b67-4a7d-bc12-f4a9e6c8d351"));

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("d8e9f1a2-3b45-47c6-bc12-e2f3a4d5b691"));

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("e8a9c5b7-4d23-49fb-a91b-c6e1f2d8b643"));

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("f1c2d4a6-5b34-42d8-9c12-e3a7b8f6d921"));
        }
    }
}
