using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseIdToExportInvoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "Categories",
                comment: "Categories in the system");

            migrationBuilder.AddColumn<Guid>(
                name: "WarehouseId",
                table: "ExportInvoices",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "Foreign key to the Warehouse");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                comment: "Name of the category ",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "Description of the category , optional",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Category identifier",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.UpdateData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("3f36e42f-799b-4d60-a96f-3f07947a66d0"),
                column: "WarehouseId",
                value: new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470"));

            migrationBuilder.UpdateData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("4db5f5f4-8be3-4ecf-98cd-6d4a4f1d2cf2"),
                column: "WarehouseId",
                value: new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470"));

            migrationBuilder.UpdateData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("8b5c1e28-1bc7-4c70-bb2e-2f2c8e5b39f5"),
                column: "WarehouseId",
                value: new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470"));

            migrationBuilder.UpdateData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("a1d1c8f3-91b0-4f62-b7b1-b1d13a7e75c8"),
                column: "WarehouseId",
                value: new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470"));

            migrationBuilder.UpdateData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("b65c7e3a-76a3-42b3-bbc2-73c1a693743e"),
                column: "WarehouseId",
                value: new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470"));

            migrationBuilder.UpdateData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("bc107ca4-2833-4f26-9d1e-36ab9080c418"),
                column: "WarehouseId",
                value: new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470"));

            migrationBuilder.UpdateData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("cae789ba-1bcd-4e32-b57c-8243fc4d8d19"),
                column: "WarehouseId",
                value: new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470"));

            migrationBuilder.UpdateData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("dcb7f1ae-b8f1-4670-93b2-5e7ff7d3c3c6"),
                column: "WarehouseId",
                value: new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470"));

            migrationBuilder.UpdateData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("f4c7e1a5-4392-4f86-92d1-0b5f3a7c46f2"),
                column: "WarehouseId",
                value: new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470"));

            migrationBuilder.UpdateData(
                table: "ExportInvoices",
                keyColumn: "Id",
                keyValue: new Guid("fe7f4c16-f76f-42e9-bbfe-6c9e6b87e33b"),
                column: "WarehouseId",
                value: new Guid("b689e5b1-8c23-462d-b931-97a7d2b40470"));

            migrationBuilder.CreateIndex(
                name: "IX_ExportInvoices_WarehouseId_InvoiceNumber",
                table: "ExportInvoices",
                columns: new[] { "WarehouseId", "InvoiceNumber" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExportInvoices_Warehouses_WarehouseId",
                table: "ExportInvoices",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportInvoices_Warehouses_WarehouseId",
                table: "ExportInvoices");

            migrationBuilder.DropIndex(
                name: "IX_ExportInvoices_WarehouseId_InvoiceNumber",
                table: "ExportInvoices");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "ExportInvoices");

            migrationBuilder.AlterTable(
                name: "Categories",
                oldComment: "Categories in the system");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldComment: "Name of the category ");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "Description of the category , optional");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Category identifier");
        }
    }
}
