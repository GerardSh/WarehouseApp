using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WarehouseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedExportInvoicesAndExportInvoiceDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ExportInvoices",
                columns: new[] { "Id", "ClientId", "Date", "InvoiceNumber" },
                values: new object[,]
                {
                    { new Guid("3f36e42f-799b-4d60-a96f-3f07947a66d0"), new Guid("e3b0c442-98fc-1c14-9afb-f4c8996fb924"), new DateTime(2025, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "EXP008" },
                    { new Guid("4db5f5f4-8be3-4ecf-98cd-6d4a4f1d2cf2"), new Guid("8a3f9b62-1e24-46f1-bc95-71f25a4c0e9a"), new DateTime(2025, 4, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "EXP003" },
                    { new Guid("8b5c1e28-1bc7-4c70-bb2e-2f2c8e5b39f5"), new Guid("f56d3c8b-92a4-4d71-82c7-6f4b9e2c1f0d"), new DateTime(2025, 4, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "EXP004" },
                    { new Guid("a1d1c8f3-91b0-4f62-b7b1-b1d13a7e75c8"), new Guid("8a3f9b62-1e24-46f1-bc95-71f25a4c0e9a"), new DateTime(2025, 4, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "EXP002" },
                    { new Guid("b65c7e3a-76a3-42b3-bbc2-73c1a693743e"), new Guid("b2d9f1c5-84e7-4c1b-92a6-3a5e7c0f8b29"), new DateTime(2025, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "EXP005" },
                    { new Guid("bc107ca4-2833-4f26-9d1e-36ab9080c418"), new Guid("f56d3c8b-92a4-4d71-82c7-6f4b9e2c1f0d"), new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "EXP009" },
                    { new Guid("cae789ba-1bcd-4e32-b57c-8243fc4d8d19"), new Guid("d9f1b25c-84e6-4c1a-92b5-3a5e8c0f7b19"), new DateTime(2025, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "EXP010" },
                    { new Guid("dcb7f1ae-b8f1-4670-93b2-5e7ff7d3c3c6"), new Guid("d9f1b25c-84e6-4c1a-92b5-3a5e8c0f7b19"), new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "EXP007" },
                    { new Guid("f4c7e1a5-4392-4f86-92d1-0b5f3a7c46f2"), new Guid("8a3f9b62-1e24-46f1-bc95-71f25a4c0e9a"), new DateTime(2025, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "EXP001" },
                    { new Guid("fe7f4c16-f76f-42e9-bbfe-6c9e6b87e33b"), new Guid("f8c3a5d7-21b4-4e9c-86f2-7d5b3a9e1c4b"), new DateTime(2025, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "EXP006" }
                });

            migrationBuilder.InsertData(
                table: "ExportInvoiceDetails",
                columns: new[] { "Id", "ExportInvoiceId", "ImportInvoiceDetailId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { new Guid("06b2a659-5a9c-4ad3-a324-55b2b06d7f27"), new Guid("4db5f5f4-8be3-4ecf-98cd-6d4a4f1d2cf2"), new Guid("c5b2f2fa-f5de-4172-b558-b8a0aa31cbd4"), 1, 99.99m },
                    { new Guid("3f218f66-7b42-429c-bb02-5cd9b7d1147b"), new Guid("cae789ba-1bcd-4e32-b57c-8243fc4d8d19"), new Guid("ebef2229-41c0-41fa-a56c-f3a9d7618ad4"), 2, 150.0m },
                    { new Guid("46c8c119-faf9-4e3c-8be1-0f3de6c287d4"), new Guid("4db5f5f4-8be3-4ecf-98cd-6d4a4f1d2cf2"), new Guid("39e3849a-b72d-4372-94f6-7e42f7c3cb35"), 3, 100.00m },
                    { new Guid("4e0d9c60-1051-4a0e-a676-bd2a3a24b1cf"), new Guid("dcb7f1ae-b8f1-4670-93b2-5e7ff7d3c3c6"), new Guid("328cc44d-d73f-4956-b2cb-dc7cd1c4f437"), 10, 105.00m },
                    { new Guid("59e1bd10-c198-4b30-b5e4-0fc97a77d8c1"), new Guid("8b5c1e28-1bc7-4c70-bb2e-2f2c8e5b39f5"), new Guid("ebef2229-41c0-41fa-a56c-f3a9d7618ad4"), 12, 150.00m },
                    { new Guid("763b9cb0-d02d-468b-83ef-1f245b599d93"), new Guid("3f36e42f-799b-4d60-a96f-3f07947a66d0"), new Guid("39e3849a-b72d-4372-94f6-7e42f7c3cb35"), 3, 100.00m },
                    { new Guid("87a64d60-e444-4b84-b88f-9d5c78d4fc72"), new Guid("b65c7e3a-76a3-42b3-bbc2-73c1a693743e"), new Guid("f44fa9ea-cf1b-4c1d-936e-b31f4d7298d3"), 3, 159.50m },
                    { new Guid("92e4e83c-2b56-40d0-aefd-f99d9ff7030b"), new Guid("bc107ca4-2833-4f26-9d1e-36ab9080c418"), new Guid("ebef2229-41c0-41fa-a56c-f3a9d7618ad4"), 3, 150.0m },
                    { new Guid("991cbf9e-c58e-4d13-9988-7ddf3de1d91e"), new Guid("b65c7e3a-76a3-42b3-bbc2-73c1a693743e"), new Guid("cf13a1ec-99fc-4f7c-a635-45f35fc72ad5"), 3, 220.00m },
                    { new Guid("a059a5c3-e759-40a1-8d02-b1a3f8c04dc2"), new Guid("a1d1c8f3-91b0-4f62-b7b1-b1d13a7e75c8"), new Guid("c5b2f2fa-f5de-4172-b558-b8a0aa31cbd4"), 2, 99.99m },
                    { new Guid("a0f0e6bd-8a18-4be8-931b-bd2928b8bbd7"), new Guid("bc107ca4-2833-4f26-9d1e-36ab9080c418"), new Guid("cf13a1ec-99fc-4f7c-a635-45f35fc72ad5"), 1, 220.00m },
                    { new Guid("b73f4d10-f2a9-4983-a96b-95ff3f3a67f0"), new Guid("f4c7e1a5-4392-4f86-92d1-0b5f3a7c46f2"), new Guid("35f5e0b1-8e2e-4e2e-b46c-e1f392ff65a4"), 5, 309.99m },
                    { new Guid("bc09320d-4e82-4b8b-89a6-f9eb69a401d6"), new Guid("fe7f4c16-f76f-42e9-bbfe-6c9e6b87e33b"), new Guid("ebef2229-41c0-41fa-a56c-f3a9d7618ad4"), 2, 150.00m },
                    { new Guid("cb884fc2-b770-4632-9a10-29b4b5e16e64"), new Guid("fe7f4c16-f76f-42e9-bbfe-6c9e6b87e33b"), new Guid("7a1bfb82-7a6e-47f9-bbbf-81093e4a2876"), 4, 330.00m },
                    { new Guid("cc98d9a1-6e55-4b1e-99a4-1e1f36b0c1c1"), new Guid("f4c7e1a5-4392-4f86-92d1-0b5f3a7c46f2"), new Guid("5b53a1d4-7b7b-4127-bdb4-388c870e7082"), 5, 209.99m },
                    { new Guid("cfcb0e87-8be3-44ef-bad6-417dc560e0e6"), new Guid("bc107ca4-2833-4f26-9d1e-36ab9080c418"), new Guid("f44fa9ea-cf1b-4c1d-936e-b31f4d7298d3"), 2, 159.5m },
                    { new Guid("d09c3c46-f247-4a7e-a7b0-0ab7b2cf8039"), new Guid("a1d1c8f3-91b0-4f62-b7b1-b1d13a7e75c8"), new Guid("5b53a1d4-7b7b-4127-bdb4-388c870e7082"), 5, 209.99m },
                    { new Guid("e2517aef-6fd1-4d08-9a20-c632582d5b32"), new Guid("4db5f5f4-8be3-4ecf-98cd-6d4a4f1d2cf2"), new Guid("b0a77ed1-4c9b-4056-a05c-f01e6be521f0"), 4, 320.00m },
                    { new Guid("e6a2b1f9-6df5-4d0e-9877-137b191c6b80"), new Guid("a1d1c8f3-91b0-4f62-b7b1-b1d13a7e75c8"), new Guid("d91fd94a-79f8-4023-9cc5-502a9ffacfd7"), 12, 69.99m },
                    { new Guid("f86c942d-c6c0-4cc5-8b33-768647f3e96e"), new Guid("4db5f5f4-8be3-4ecf-98cd-6d4a4f1d2cf2"), new Guid("cf13a1ec-99fc-4f7c-a635-45f35fc72ad5"), 10, 220.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("06b2a659-5a9c-4ad3-a324-55b2b06d7f27"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("3f218f66-7b42-429c-bb02-5cd9b7d1147b"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("46c8c119-faf9-4e3c-8be1-0f3de6c287d4"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("4e0d9c60-1051-4a0e-a676-bd2a3a24b1cf"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("59e1bd10-c198-4b30-b5e4-0fc97a77d8c1"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("763b9cb0-d02d-468b-83ef-1f245b599d93"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("87a64d60-e444-4b84-b88f-9d5c78d4fc72"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("92e4e83c-2b56-40d0-aefd-f99d9ff7030b"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("991cbf9e-c58e-4d13-9988-7ddf3de1d91e"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("a059a5c3-e759-40a1-8d02-b1a3f8c04dc2"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("a0f0e6bd-8a18-4be8-931b-bd2928b8bbd7"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("b73f4d10-f2a9-4983-a96b-95ff3f3a67f0"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("bc09320d-4e82-4b8b-89a6-f9eb69a401d6"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("cb884fc2-b770-4632-9a10-29b4b5e16e64"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("cc98d9a1-6e55-4b1e-99a4-1e1f36b0c1c1"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("cfcb0e87-8be3-44ef-bad6-417dc560e0e6"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("d09c3c46-f247-4a7e-a7b0-0ab7b2cf8039"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("e2517aef-6fd1-4d08-9a20-c632582d5b32"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("e6a2b1f9-6df5-4d0e-9877-137b191c6b80"));

            migrationBuilder.DeleteData(
                table: "ExportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("f86c942d-c6c0-4cc5-8b33-768647f3e96e"));

            migrationBuilder.DeleteData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("3f36e42f-799b-4d60-a96f-3f07947a66d0"));

            migrationBuilder.DeleteData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("4db5f5f4-8be3-4ecf-98cd-6d4a4f1d2cf2"));

            migrationBuilder.DeleteData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("8b5c1e28-1bc7-4c70-bb2e-2f2c8e5b39f5"));

            migrationBuilder.DeleteData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("a1d1c8f3-91b0-4f62-b7b1-b1d13a7e75c8"));

            migrationBuilder.DeleteData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("b65c7e3a-76a3-42b3-bbc2-73c1a693743e"));

            migrationBuilder.DeleteData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("bc107ca4-2833-4f26-9d1e-36ab9080c418"));

            migrationBuilder.DeleteData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("cae789ba-1bcd-4e32-b57c-8243fc4d8d19"));

            migrationBuilder.DeleteData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("dcb7f1ae-b8f1-4670-93b2-5e7ff7d3c3c6"));

            migrationBuilder.DeleteData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("f4c7e1a5-4392-4f86-92d1-0b5f3a7c46f2"));

            migrationBuilder.DeleteData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("fe7f4c16-f76f-42e9-bbfe-6c9e6b87e33b"));
        }
    }
}
