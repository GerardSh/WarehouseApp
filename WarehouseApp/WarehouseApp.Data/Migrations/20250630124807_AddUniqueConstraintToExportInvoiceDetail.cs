using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToExportInvoiceDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExportInvoiceDetails_ExportInvoiceId",
                table: "ExportInvoiceDetails");

            migrationBuilder.CreateIndex(
                name: "IX_ExportInvoiceDetails_ExportInvoiceId_ImportInvoiceDetailId",
                table: "ExportInvoiceDetails",
                columns: new[] { "ExportInvoiceId", "ImportInvoiceDetailId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExportInvoiceDetails_ExportInvoiceId_ImportInvoiceDetailId",
                table: "ExportInvoiceDetails");

            migrationBuilder.CreateIndex(
                name: "IX_ExportInvoiceDetails_ExportInvoiceId",
                table: "ExportInvoiceDetails",
                column: "ExportInvoiceId");
        }
    }
}
