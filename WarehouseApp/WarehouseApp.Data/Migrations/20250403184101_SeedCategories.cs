using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WarehouseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("26ea2ad2-8d2c-4c43-939f-d7d818572aff"), "Accessories to complement mobile devices, including power banks and docking stations.", "Mobile Accessories" },
                    { new Guid("314820d6-a4d4-4873-8dec-95e5a6904a8e"), "Technology that you wear, including smartwatches and wireless earbuds.", "Wearable Tech" },
                    { new Guid("31bf5b32-a3f7-4e37-8230-2202a081ea87"), "Products related to computers and accessories such as keyboards, mice, and gaming gear.", "Computers & Peripherals" },
                    { new Guid("3daadd5e-f5c9-4222-9878-1a02faa44339"), "Furniture and accessories designed for improving office comfort and productivity.", "Office Furniture & Accessories" },
                    { new Guid("dde109fb-71c5-4f59-8810-3bf4057c5351"), "Storage solutions and backup devices for your data, such as external hard drives and SSDs.", "Storage & Backup" },
                    { new Guid("e3edc37f-6ba9-4616-b0c5-98c33451bcdb"), "Small appliances for use at home such as coffee makers, speakers, and chargers.", "Home Appliances" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("26ea2ad2-8d2c-4c43-939f-d7d818572aff"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("314820d6-a4d4-4873-8dec-95e5a6904a8e"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("31bf5b32-a3f7-4e37-8230-2202a081ea87"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("3daadd5e-f5c9-4222-9878-1a02faa44339"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("dde109fb-71c5-4f59-8810-3bf4057c5351"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("e3edc37f-6ba9-4616-b0c5-98c33451bcdb"));
        }
    }
}
