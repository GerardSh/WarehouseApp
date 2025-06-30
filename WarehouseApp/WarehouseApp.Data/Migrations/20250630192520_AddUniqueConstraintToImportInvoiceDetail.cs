using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToImportInvoiceDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImportInvoiceDetails_ImportInvoiceId",
                table: "ImportInvoiceDetails");

            migrationBuilder.CreateIndex(
                name: "IX_ImportInvoiceDetails_ImportInvoiceId_ProductId",
                table: "ImportInvoiceDetails",
                columns: new[] { "ImportInvoiceId", "ProductId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImportInvoiceDetails_ImportInvoiceId_ProductId",
                table: "ImportInvoiceDetails");

            migrationBuilder.CreateIndex(
                name: "IX_ImportInvoiceDetails_ImportInvoiceId",
                table: "ImportInvoiceDetails",
                column: "ImportInvoiceId");
        }
    }
}
