namespace WarehouseApp.Web.ViewModels.ExportInvoice
{
    public class ExportInvoiceDetailDetailsViewModel
    {
        public string ImportInvoiceNumber { get; set; } = null!;

        public string ProductName { get; set; } = null!;

        public string? ProductDescription { get; set; }

        public string CategoryName { get; set; } = null!;

        public string? CategoryDescription { get; set; }

        public int Quantity { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? Total { get; set; }
    }
}
