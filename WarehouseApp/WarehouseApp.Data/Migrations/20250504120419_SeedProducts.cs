using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WarehouseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("72c4bc6d-72f5-4b43-b299-cb1bcaa4cd93"), new Guid("31bf5b32-a3f7-4e37-8230-2202a081ea87"), "High-quality mechanical keyboard with customizable RGB lighting.", "Mechanical Keyboard" },
                    { new Guid("a0b5d4c3-7e92-4f81-bc2d-5f3a7d1e4689"), new Guid("3daadd5e-f5c9-4222-9878-1a02faa44339"), "Adjustable office chair with lumbar support.", "Ergonomic Office Chair" },
                    { new Guid("a4b3c7d5-9e1f-4f86-bc2d-0b5d4e3c7981"), new Guid("e3edc37f-6ba9-4616-b0c5-98c33451bcdb"), "Compact coffee maker for brewing on the go.", "Portable Coffee Maker" },
                    { new Guid("a5b3d7c2-9e1f-4f86-bc2d-0b5d4c3e7981"), new Guid("314820d6-a4d4-4873-8dec-95e5a6904a8e"), "Next-gen VR headset with high-resolution display and tracking.", "VR Headset" },
                    { new Guid("a6a5c1b8-d5bc-46b5-b75c-0152e35e344e"), new Guid("31bf5b32-a3f7-4e37-8230-2202a081ea87"), "Ergonomic wireless mouse with adjustable DPI settings.", "Wireless Mouse" },
                    { new Guid("b4c9d2e7-1a3f-4856-bc91-f5d3a0b7e462"), new Guid("26ea2ad2-8d2c-4c43-939f-d7d818572aff"), "Fast wireless charger compatible with Qi-enabled devices.", "Wireless Charger" },
                    { new Guid("b5c4d7e2-3a92-4f81-9b1f-7d3a0b5e4682"), new Guid("dde109fb-71c5-4f59-8810-3bf4057c5351"), "2TB external hard drive for secure data backup.", "External Hard Drive" },
                    { new Guid("b5e21436-3c45-4f52-a854-874bcf356f9d"), new Guid("3daadd5e-f5c9-4222-9878-1a02faa44339"), "Adjustable aluminum laptop stand for improved ergonomics.", "Laptop Stand" },
                    { new Guid("b7c4d5e2-3a92-4f86-91b1-7d3a0b5e4683"), new Guid("26ea2ad2-8d2c-4c43-939f-d7d818572aff"), "20000mAh portable power bank with fast charging capabilities.", "Portable Power Bank" },
                    { new Guid("c3a9d582-74b6-4512-91a5-2643a1b682be"), new Guid("31bf5b32-a3f7-4e37-8230-2202a081ea87"), "Multi-port USB-C hub with HDMI, Ethernet, and USB ports.", "USB-C Docking Station" },
                    { new Guid("c8f3a7d2-1b6e-4c5d-92f4-a0b5d4e7c9f1"), new Guid("31bf5b32-a3f7-4e37-8230-2202a081ea87"), "27-inch 4K UHD monitor with HDR and IPS panel.", "4K Monitor" },
                    { new Guid("d1c7e4a5-3b92-4f86-bc7d-1a0b5f3e4826"), new Guid("314820d6-a4d4-4873-8dec-95e5a6904a8e"), "Fitness-focused smartwatch with heart rate monitoring.", "Smartwatch" },
                    { new Guid("d2c7e5a4-3b91-4f86-92d1-0b5a3f7c4682"), new Guid("314820d6-a4d4-4873-8dec-95e5a6904a8e"), "True wireless earbuds with active noise cancellation.", "Noise-Canceling Earbuds" },
                    { new Guid("d4c7e1a5-3b92-4f86-92d1-0b5f3a7c4682"), new Guid("31bf5b32-a3f7-4e37-8230-2202a081ea87"), "Professional-grade graphics tablet with pressure-sensitive stylus.", "Graphics Tablet" },
                    { new Guid("d9b37f82-0b4f-4c92-8821-f4b3b5dcd045"), new Guid("31bf5b32-a3f7-4e37-8230-2202a081ea87"), "Over-ear gaming headset with noise cancellation and surround sound.", "Gaming Headset" },
                    { new Guid("e7a2c5d4-3b9f-4871-82c1-d5b4a0f6e398"), new Guid("e3edc37f-6ba9-4616-b0c5-98c33451bcdb"), "Compact Bluetooth speaker with deep bass and long battery life.", "Bluetooth Speaker" },
                    { new Guid("ed2b3c7f-8a45-4d9e-80f4-b1c7d2a0f9b2"), new Guid("dde109fb-71c5-4f59-8810-3bf4057c5351"), "1TB portable SSD with high-speed data transfer.", "Portable SSD" },
                    { new Guid("f1a3b5c7-92d4-4f86-8b1d-0b5e4c3d7982"), new Guid("e3edc37f-6ba9-4616-b0c5-98c33451bcdb"), "Wi-Fi-enabled smart light bulb with color-changing options.", "Smart Light Bulb" },
                    { new Guid("f1a5b3c7-74d9-4e8f-92b1-c3d2a0b5f8e6"), new Guid("26ea2ad2-8d2c-4c43-939f-d7d818572aff"), "Flexible gooseneck smartphone holder for hands-free use.", "Smartphone Holder" },
                    { new Guid("f3a5b7d2-1e9c-4f86-bc2d-0b5d4c3e7891"), new Guid("3daadd5e-f5c9-4222-9878-1a02faa44339"), "Premium mechanical pencil set for precise writing and drawing.", "Mechanical Pencil Set" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("72c4bc6d-72f5-4b43-b299-cb1bcaa4cd93"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a0b5d4c3-7e92-4f81-bc2d-5f3a7d1e4689"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a4b3c7d5-9e1f-4f86-bc2d-0b5d4e3c7981"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a5b3d7c2-9e1f-4f86-bc2d-0b5d4c3e7981"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a6a5c1b8-d5bc-46b5-b75c-0152e35e344e"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b4c9d2e7-1a3f-4856-bc91-f5d3a0b7e462"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b5c4d7e2-3a92-4f81-9b1f-7d3a0b5e4682"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b5e21436-3c45-4f52-a854-874bcf356f9d"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b7c4d5e2-3a92-4f86-91b1-7d3a0b5e4683"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("c3a9d582-74b6-4512-91a5-2643a1b682be"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("c8f3a7d2-1b6e-4c5d-92f4-a0b5d4e7c9f1"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d1c7e4a5-3b92-4f86-bc7d-1a0b5f3e4826"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d2c7e5a4-3b91-4f86-92d1-0b5a3f7c4682"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d4c7e1a5-3b92-4f86-92d1-0b5f3a7c4682"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d9b37f82-0b4f-4c92-8821-f4b3b5dcd045"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("e7a2c5d4-3b9f-4871-82c1-d5b4a0f6e398"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("ed2b3c7f-8a45-4d9e-80f4-b1c7d2a0f9b2"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f1a3b5c7-92d4-4f86-8b1d-0b5e4c3d7982"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f1a5b3c7-74d9-4e8f-92b1-c3d2a0b5f8e6"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f3a5b7d2-1e9c-4f86-bc2d-0b5d4c3e7891"));
        }
    }
}
