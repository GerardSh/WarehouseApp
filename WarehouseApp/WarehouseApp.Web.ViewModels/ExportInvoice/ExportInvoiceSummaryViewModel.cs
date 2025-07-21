namespace WarehouseApp.Web.ViewModels.ExportInvoice
{
    public class ExportInvoiceSummaryViewModel
    {
        public string Id { get; set; } = null!;

        public string InvoiceNumber { get; set; } = null!;

        public string Date { get; set; } = null!;

        public string ClientName { get; set; } = null!;

        public string ExportedProductsCount { get; set; } = null!;
    }
}
