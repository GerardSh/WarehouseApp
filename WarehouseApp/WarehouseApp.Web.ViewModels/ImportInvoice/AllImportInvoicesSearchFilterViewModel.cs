using WarehouseApp.Web.ViewModels.Warehouse;

namespace WarehouseApp.Web.ViewModels.ImportInvoice
{
    public class AllImportInvoicesSearchFilterViewModel
    {
        public IEnumerable<ImportInvoiceSummaryViewModel> Invoices { get; set; }
                = new List<ImportInvoiceSummaryViewModel>();

        public string? SearchQuery { get; set; }

        public string? SupplierName { get; set; }

        public string? YearFilter { get; set; }

        public int? CurrentPage { get; set; } = 1;

        public int? EntitiesPerPage { get; set; } = 5;

        public int TotalItemsBeforePagination { get; set; }

        public int? TotalPages { get; set; }

        public int TotalInvoices { get; set; }
    }
}
