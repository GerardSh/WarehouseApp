namespace WarehouseApp.Web.ViewModels.ImportInvoice
{
    public class ImportInvoiceSummaryViewModel
    {
        public string Id { get; set; } = null!;

        public string InvoiceNumber { get; set; } = null!;

        public string Date { get; set; } = null!;

        public string SupplierName { get; set; } = null!;

        public string ProductCount { get; set; } = null!;
    }
}
