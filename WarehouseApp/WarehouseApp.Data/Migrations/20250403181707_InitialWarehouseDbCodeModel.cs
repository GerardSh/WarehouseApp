using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialWarehouseDbCodeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Client identifier"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "Client's name"),
                    Address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "Client's address"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true, comment: "Client's phone number"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Client's email address")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                },
                comment: "Clients in the system");

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Warehouse identifier"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "Warehouse name"),
                    Address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "Warehouse address"),
                    Size = table.Column<double>(type: "float", nullable: true, comment: "Warehouse size in square meters (m²)"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Shows the date of when the warehouse record was created"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Shows if warehouse is deleted")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                },
                comment: "Warehouses in the system");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Product identifier"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Name of the product"),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, comment: "Description of the product, optional"),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to the Category")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Products available in the warehouse");

            migrationBuilder.CreateTable(
                name: "ExportInvoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Export invoice identifier"),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Export invoice number"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Date of the export invoice"),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to the Client")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExportInvoices_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Export invoices in the system");

            migrationBuilder.CreateTable(
                name: "ImportInvoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Import invoice identifier"),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Unique import invoice number per warehouse"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Date of the import invoice"),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to the Client"),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to the Warehouse")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportInvoices_Clients_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImportInvoices_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                },
                comment: "Import invoices in the system");

            migrationBuilder.CreateTable(
                name: "ImportInvoiceDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Import invoice detail identifier"),
                    ImportInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to the ImportInvoice"),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to the Product"),
                    Quantity = table.Column<int>(type: "int", nullable: false, comment: "Quantity of the product in this import invoice detail"),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true, comment: "Unit price of the product in this import invoice detail, optional")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportInvoiceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportInvoiceDetails_ImportInvoices_ImportInvoiceId",
                        column: x => x.ImportInvoiceId,
                        principalTable: "ImportInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImportInvoiceDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Details about the imported products");

            migrationBuilder.CreateTable(
                name: "ExportInvoiceDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Export invoice detail identifier"),
                    ExportInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to the ExportInvoice"),
                    ImportInvoiceDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to the ImportInvoiceDetail"),
                    Quantity = table.Column<int>(type: "int", nullable: false, comment: "Quantity of the product in this export invoice detail"),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true, comment: "Unit price of the product in this export invoice detail")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportInvoiceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExportInvoiceDetails_ExportInvoices_ExportInvoiceId",
                        column: x => x.ExportInvoiceId,
                        principalTable: "ExportInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExportInvoiceDetails_ImportInvoiceDetails_ImportInvoiceDetailId",
                        column: x => x.ImportInvoiceDetailId,
                        principalTable: "ImportInvoiceDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Details about the exported products");

            migrationBuilder.CreateIndex(
                name: "IX_ExportInvoiceDetails_ExportInvoiceId",
                table: "ExportInvoiceDetails",
                column: "ExportInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportInvoiceDetails_ImportInvoiceDetailId",
                table: "ExportInvoiceDetails",
                column: "ImportInvoiceDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportInvoices_ClientId",
                table: "ExportInvoices",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportInvoiceDetails_ImportInvoiceId",
                table: "ImportInvoiceDetails",
                column: "ImportInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportInvoiceDetails_ProductId",
                table: "ImportInvoiceDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportInvoices_SupplierId",
                table: "ImportInvoices",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportInvoices_WarehouseId_InvoiceNumber",
                table: "ImportInvoices",
                columns: new[] { "WarehouseId", "InvoiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExportInvoiceDetails");

            migrationBuilder.DropTable(
                name: "ExportInvoices");

            migrationBuilder.DropTable(
                name: "ImportInvoiceDetails");

            migrationBuilder.DropTable(
                name: "ImportInvoices");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
