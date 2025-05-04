using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WarehouseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedImportInvoicesAndImportInvoiceDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ImportInvoices",
                columns: new[] { "Id", "Date", "InvoiceNumber", "SupplierId", "WarehouseId" },
                values: new object[,]
                {
                    { new Guid("4d3e97b5-119e-4cf2-8b78-4f5478a5a401"), new DateTime(2025, 4, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV001", new Guid("e3b0c442-98fc-1c14-9afb-f4c8996fb924"), new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470") },
                    { new Guid("8e9d59b2-f682-4e4b-8230-538eef5772dc"), new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV005", new Guid("b2d9f1c5-84e7-4c1b-92a6-3a5e7c0f8b29"), new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470") },
                    { new Guid("a1cfe4a7-dbc3-4c8f-9582-02efc6ea47f3"), new DateTime(2025, 4, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV003", new Guid("f56d3c8b-92a4-4d71-82c7-6f4b9e2c1f0d"), new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470") },
                    { new Guid("a38f5c8b-c8d6-4590-bf5f-f5d616f1c1b7"), new DateTime(2025, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV009", new Guid("f56d3c8b-92a4-4d71-82c7-6f4b9e2c1f0d"), new Guid("be8f00a5-682d-4b43-9734-d3e17078cb52") },
                    { new Guid("b2d5cb4e-fc88-4c9f-9e9e-4a6a3f4e70d4"), new DateTime(2025, 4, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV004", new Guid("a1c4b9e5-7f28-48c1-b62d-4f3a5d7e92b8"), new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470") },
                    { new Guid("c4f631f3-16a1-47a4-8987-5a4b9a24b4d6"), new DateTime(2025, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV010", new Guid("a1c4b9e5-7f28-48c1-b62d-4f3a5d7e92b8"), new Guid("be8f00a5-682d-4b43-9734-d3e17078cb52") },
                    { new Guid("d9784351-b2f6-44e5-ae5c-45615df7a102"), new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV002", new Guid("3b12c292-b084-491b-9b3d-3e01b7ff2eaf"), new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470") },
                    { new Guid("e50e7e92-b28f-47f7-99f3-ff442f234cb2"), new DateTime(2025, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV007", new Guid("e3b0c442-98fc-1c14-9afb-f4c8996fb924"), new Guid("be8f00a5-682d-4b43-9734-d3e17078cb52") },
                    { new Guid("f8ed3bb4-d255-4928-8a29-c24e045f302d"), new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV008", new Guid("b3e2c5a1-7d84-4f6b-91c2-5a7f3b8e9d4f"), new Guid("be8f00a5-682d-4b43-9734-d3e17078cb52") },
                    { new Guid("fa61f3b4-e46f-496e-b90f-bd470f5b68ac"), new DateTime(2025, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV006", new Guid("8a3f9b62-1e24-46f1-bc95-71f25a4c0e9a"), new Guid("be8f00a5-682d-4b43-9734-d3e17078cb52") }
                });

            migrationBuilder.InsertData(
                table: "ImportInvoiceDetails",
                columns: new[] { "Id", "ImportInvoiceId", "ProductId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { new Guid("1a396e47-1ff1-4f8b-b8c9-f3b82b3a8c83"), new Guid("e50e7e92-b28f-47f7-99f3-ff442f234cb2"), new Guid("f1a5b3c7-74d9-4e8f-92b1-c3d2a0b5f8e6"), 15, 149.99m },
                    { new Guid("278b0a44-5c4e-42ff-a80f-dcc4b9acb306"), new Guid("fa61f3b4-e46f-496e-b90f-bd470f5b68ac"), new Guid("b7c4d5e2-3a92-4f86-91b1-7d3a0b5e4683"), 4, 120.00m },
                    { new Guid("328cc44d-d73f-4956-b2cb-dc7cd1c4f437"), new Guid("b2d5cb4e-fc88-4c9f-9e9e-4a6a3f4e70d4"), new Guid("a0b5d4c3-7e92-4f81-bc2d-5f3a7d1e4689"), 10, 95.00m },
                    { new Guid("35f5e0b1-8e2e-4e2e-b46c-e1f392ff65a4"), new Guid("4d3e97b5-119e-4cf2-8b78-4f5478a5a401"), new Guid("72c4bc6d-72f5-4b43-b299-cb1bcaa4cd93"), 5, 299.99m },
                    { new Guid("3833b38d-828d-47ae-90bc-7fe2f21cb0cc"), new Guid("e50e7e92-b28f-47f7-99f3-ff442f234cb2"), new Guid("a6a5c1b8-d5bc-46b5-b75c-0152e35e344e"), 20, 349.99m },
                    { new Guid("39e3849a-b72d-4372-94f6-7e42f7c3cb35"), new Guid("8e9d59b2-f682-4e4b-8230-538eef5772dc"), new Guid("d4c7e1a5-3b92-4f86-92d1-0b5f3a7c4682"), 9, 90.00m },
                    { new Guid("3bafbb58-56e0-4bba-9ab5-d70832ff56f4"), new Guid("c4f631f3-16a1-47a4-8987-5a4b9a24b4d6"), new Guid("72c4bc6d-72f5-4b43-b299-cb1bcaa4cd93"), 8, 299.99m },
                    { new Guid("4532b6e6-53cf-44a1-9392-8a85f2c89901"), new Guid("b2d5cb4e-fc88-4c9f-9e9e-4a6a3f4e70d4"), new Guid("a5b3d7c2-9e1f-4f86-bc2d-0b5d4c3e7981"), 6, 65.00m },
                    { new Guid("4b31e9db-d573-4d58-bd9f-0df688f5cd60"), new Guid("e50e7e92-b28f-47f7-99f3-ff442f234cb2"), new Guid("c8f3a7d2-1b6e-4c5d-92f4-a0b5d4e7c9f1"), 5, 259.99m },
                    { new Guid("5b53a1d4-7b7b-4127-bdb4-388c870e7082"), new Guid("4d3e97b5-119e-4cf2-8b78-4f5478a5a401"), new Guid("a6a5c1b8-d5bc-46b5-b75c-0152e35e344e"), 10, 199.99m },
                    { new Guid("7a1bfb82-7a6e-47f9-bbbf-81093e4a2876"), new Guid("b2d5cb4e-fc88-4c9f-9e9e-4a6a3f4e70d4"), new Guid("72c4bc6d-72f5-4b43-b299-cb1bcaa4cd93"), 7, 320.00m },
                    { new Guid("7c2e50c5-28ba-4234-b1ae-e1d70962b4ea"), new Guid("c4f631f3-16a1-47a4-8987-5a4b9a24b4d6"), new Guid("f1a3b5c7-92d4-4f86-8b1d-0b5e4c3d7982"), 15, 59.99m },
                    { new Guid("b0a77ed1-4c9b-4056-a05c-f01e6be521f0"), new Guid("8e9d59b2-f682-4e4b-8230-538eef5772dc"), new Guid("72c4bc6d-72f5-4b43-b299-cb1bcaa4cd93"), 4, 310.00m },
                    { new Guid("b319cb60-043f-462f-8a06-6a17b66de6ea"), new Guid("e50e7e92-b28f-47f7-99f3-ff442f234cb2"), new Guid("d4c7e1a5-3b92-4f86-92d1-0b5f3a7c4682"), 12, 449.99m },
                    { new Guid("b3c5a697-dba0-4b5e-bb61-7f2c455e7ac1"), new Guid("a38f5c8b-c8d6-4590-bf5f-f5d616f1c1b7"), new Guid("a6a5c1b8-d5bc-46b5-b75c-0152e35e344e"), 20, 109.99m },
                    { new Guid("bb93b12f-c8b1-4d2e-a53b-1e05f0e6fe0a"), new Guid("f8ed3bb4-d255-4928-8a29-c24e045f302d"), new Guid("72c4bc6d-72f5-4b43-b299-cb1bcaa4cd93"), 3, 289.99m },
                    { new Guid("c5b2f2fa-f5de-4172-b558-b8a0aa31cbd4"), new Guid("d9784351-b2f6-44e5-ae5c-45615df7a102"), new Guid("a0b5d4c3-7e92-4f81-bc2d-5f3a7d1e4689"), 3, 89.99m },
                    { new Guid("cbf676be-59d0-4db7-b0ec-3cd7d32f2b73"), new Guid("e50e7e92-b28f-47f7-99f3-ff442f234cb2"), new Guid("b4c9d2e7-1a3f-4856-bc91-f5d3a0b7e462"), 10, 199.99m },
                    { new Guid("cf13a1ec-99fc-4f7c-a635-45f35fc72ad5"), new Guid("b2d5cb4e-fc88-4c9f-9e9e-4a6a3f4e70d4"), new Guid("d4c7e1a5-3b92-4f86-92d1-0b5f3a7c4682"), 15, 210.00m },
                    { new Guid("d1a60174-118f-46b5-91a2-16a55e6a0b44"), new Guid("e50e7e92-b28f-47f7-99f3-ff442f234cb2"), new Guid("e7a2c5d4-3b9f-4871-82c1-d5b4a0f6e398"), 8, 119.50m },
                    { new Guid("d91fd94a-79f8-4023-9cc5-502a9ffacfd7"), new Guid("a1cfe4a7-dbc3-4c8f-9582-02efc6ea47f3"), new Guid("ed2b3c7f-8a45-4d9e-80f4-b1c7d2a0f9b2"), 12, 59.99m },
                    { new Guid("ebef2229-41c0-41fa-a56c-f3a9d7618ad4"), new Guid("b2d5cb4e-fc88-4c9f-9e9e-4a6a3f4e70d4"), new Guid("f1a5b3c7-74d9-4e8f-92b1-c3d2a0b5f8e6"), 20, 140.00m },
                    { new Guid("ed547c62-e8d2-413f-9c7d-cfffc8a9da6c"), new Guid("f8ed3bb4-d255-4928-8a29-c24e045f302d"), new Guid("c3a9d582-74b6-4512-91a5-2643a1b682be"), 10, 159.99m },
                    { new Guid("f44fa9ea-cf1b-4c1d-936e-b31f4d7298d3"), new Guid("d9784351-b2f6-44e5-ae5c-45615df7a102"), new Guid("a4b3c7d5-9e1f-4f86-bc2d-0b5d4e3c7981"), 8, 149.50m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("1a396e47-1ff1-4f8b-b8c9-f3b82b3a8c83"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("278b0a44-5c4e-42ff-a80f-dcc4b9acb306"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("328cc44d-d73f-4956-b2cb-dc7cd1c4f437"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("35f5e0b1-8e2e-4e2e-b46c-e1f392ff65a4"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("3833b38d-828d-47ae-90bc-7fe2f21cb0cc"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("39e3849a-b72d-4372-94f6-7e42f7c3cb35"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("3bafbb58-56e0-4bba-9ab5-d70832ff56f4"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("4532b6e6-53cf-44a1-9392-8a85f2c89901"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("4b31e9db-d573-4d58-bd9f-0df688f5cd60"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("5b53a1d4-7b7b-4127-bdb4-388c870e7082"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("7a1bfb82-7a6e-47f9-bbbf-81093e4a2876"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("7c2e50c5-28ba-4234-b1ae-e1d70962b4ea"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("b0a77ed1-4c9b-4056-a05c-f01e6be521f0"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("b319cb60-043f-462f-8a06-6a17b66de6ea"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("b3c5a697-dba0-4b5e-bb61-7f2c455e7ac1"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("bb93b12f-c8b1-4d2e-a53b-1e05f0e6fe0a"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("c5b2f2fa-f5de-4172-b558-b8a0aa31cbd4"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("cbf676be-59d0-4db7-b0ec-3cd7d32f2b73"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("cf13a1ec-99fc-4f7c-a635-45f35fc72ad5"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("d1a60174-118f-46b5-91a2-16a55e6a0b44"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("d91fd94a-79f8-4023-9cc5-502a9ffacfd7"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("ebef2229-41c0-41fa-a56c-f3a9d7618ad4"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("ed547c62-e8d2-413f-9c7d-cfffc8a9da6c"));

            migrationBuilder.DeleteData(
                table: "ImportInvoiceDetails",
                keyColumn: "Id",
                keyValue: new Guid("f44fa9ea-cf1b-4c1d-936e-b31f4d7298d3"));

            migrationBuilder.DeleteData(
                table: "ImportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("4d3e97b5-119e-4cf2-8b78-4f5478a5a401"));

            migrationBuilder.DeleteData(
                table: "ImportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("8e9d59b2-f682-4e4b-8230-538eef5772dc"));

            migrationBuilder.DeleteData(
                table: "ImportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("a1cfe4a7-dbc3-4c8f-9582-02efc6ea47f3"));

            migrationBuilder.DeleteData(
                table: "ImportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("a38f5c8b-c8d6-4590-bf5f-f5d616f1c1b7"));

            migrationBuilder.DeleteData(
                table: "ImportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("b2d5cb4e-fc88-4c9f-9e9e-4a6a3f4e70d4"));

            migrationBuilder.DeleteData(
                table: "ImportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("c4f631f3-16a1-47a4-8987-5a4b9a24b4d6"));

            migrationBuilder.DeleteData(
                table: "ImportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("d9784351-b2f6-44e5-ae5c-45615df7a102"));

            migrationBuilder.DeleteData(
                table: "ImportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("e50e7e92-b28f-47f7-99f3-ff442f234cb2"));

            migrationBuilder.DeleteData(
                table: "ImportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("f8ed3bb4-d255-4928-8a29-c24e045f302d"));

            migrationBuilder.DeleteData(
                table: "ImportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("fa61f3b4-e46f-496e-b90f-bd470f5b68ac"));
        }
    }
}
