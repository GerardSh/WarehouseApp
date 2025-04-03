using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WarehouseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedClients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Address", "Email", "Name", "PhoneNumber" },
                values: new object[,]
                {
                    { new Guid("3b12c292-b084-491b-9b3d-3e01b7ff2eaf"), "456 Tech Park, San Francisco, USA", "contact@innovelectronics.com", "Innovative Electronics Corp.", "+1-415-555-0456" },
                    { new Guid("8a3f9b62-1e24-46f1-bc95-71f25a4c0e9a"), "101 Digital Health Park, Berlin, Germany", "sales@smarttechhealth.com", "SmartTech Healthcare Solutions", "+49-30-5557-0224" },
                    { new Guid("a1c4b9e5-7f28-48c1-b62d-4f3a5d7e92b8"), "22 Wall Street, New York, USA", "hello@fintechinnov.com", "FinTech Innovations", "+1-646-555-0876" },
                    { new Guid("ac5b9f38-712d-456b-849f-b7a3e49c5d7e"), "67 Learning Lane, Toronto, Canada", "info@edutechinnovations.com", "EduTech Innovations", "+1-416-555-0789" },
                    { new Guid("b2d9f1c5-84e7-4c1b-92a6-3a5e7c0f8b29"), "99 Builder's Road, Dubai, UAE", "projects@constructtechmasters.com", "Construction Tech Masters", "+971-4-555-0198" },
                    { new Guid("b3e2c5a1-7d84-4f6b-91c2-5a7f3b8e9d4f"), "88 Auto Innovations Blvd, Detroit, USA", "service@autotechsystems.com", "AutoTech Systems", "+1-313-555-0912" },
                    { new Guid("d9f1b25c-84e6-4c1a-92b5-3a5e8c0f7b19"), "55 E-commerce Plaza, Paris, France", "orders@retailtechsolutions.com", "Retail Tech Solutions", "+33-1-5557-0145" },
                    { new Guid("e3b0c442-98fc-1c14-9afb-f4c8996fb924"), "123 Silicon Valley Blvd, San Jose, USA", "info@techsolutions.com", "Tech Solutions Ltd.", "+1-408-555-0123" },
                    { new Guid("f56d3c8b-92a4-4d71-82c7-6f4b9e2c1f0d"), "789 Technology Ave, London, UK", "support@globalittraders.com", "Global IT Traders", "+44-20-7946-0301" },
                    { new Guid("f8c3a5d7-21b4-4e9c-86f2-7d5b3a9e1c4b"), "77 Green Field Ave, Sydney, Australia", "contact@agrotechsolutions.com", "AgroTech Solutions", "+61-2-5557-0332" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: new Guid("3b12c292-b084-491b-9b3d-3e01b7ff2eaf"));

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: new Guid("8a3f9b62-1e24-46f1-bc95-71f25a4c0e9a"));

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: new Guid("a1c4b9e5-7f28-48c1-b62d-4f3a5d7e92b8"));

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: new Guid("ac5b9f38-712d-456b-849f-b7a3e49c5d7e"));

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: new Guid("b2d9f1c5-84e7-4c1b-92a6-3a5e7c0f8b29"));

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: new Guid("b3e2c5a1-7d84-4f6b-91c2-5a7f3b8e9d4f"));

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: new Guid("d9f1b25c-84e6-4c1a-92b5-3a5e8c0f7b19"));

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: new Guid("e3b0c442-98fc-1c14-9afb-f4c8996fb924"));

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: new Guid("f56d3c8b-92a4-4d71-82c7-6f4b9e2c1f0d"));

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: new Guid("f8c3a5d7-21b4-4e9c-86f2-7d5b3a9e1c4b"));
        }
    }
}
