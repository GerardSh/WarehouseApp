namespace WarehouseApp.Web.ViewModels.ImportInvoice
{
    public class ImportInvoiceDetailsViewModel
    {
        public Guid Id { get; set; }

        public string InvoiceNumber { get; set; } = null!;

        public DateTime Date { get; set; }

        public string SupplierName { get; set; } = null!;

        public string SupplierAddress { get; set; } = null!;

        public string? SupplierPhone { get; set; }

        public string? SupplierEmail { get; set; }

        public List<ImportInvoiceDetailDetailsViewModel> Products { get; set; }
                = new List<ImportInvoiceDetailDetailsViewModel>();
    }
}
