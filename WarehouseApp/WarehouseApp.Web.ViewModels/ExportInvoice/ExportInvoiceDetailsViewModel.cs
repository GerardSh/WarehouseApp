namespace WarehouseApp.Web.ViewModels.ExportInvoice
{
    public class ExportInvoiceDetailsViewModel
    {
        public Guid Id { get; set; }

        public string InvoiceNumber { get; set; } = null!;

        public DateTime Date { get; set; }

        public string ClientName { get; set; } = null!;

        public string ClientAddress { get; set; } = null!;

        public string? ClientPhone { get; set; }

        public string? ClientEmail { get; set; }

        public List<ExportInvoiceDetailDetailsViewModel> ExportedProducts { get; set; }
                = new List<ExportInvoiceDetailDetailsViewModel>();
    }
}
