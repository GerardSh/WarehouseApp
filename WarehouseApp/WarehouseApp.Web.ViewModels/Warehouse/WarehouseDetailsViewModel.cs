namespace WarehouseApp.Web.ViewModels.Warehouse
{
    public class WarehouseDetailsViewModel
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string CreatedByUser { get; set; } = null!;

        public bool IsUserOwner { get; set; }

        public string CreatedDate { get; set; } = null!;

        public string? Size { get; set; }

        public int TotalImportInvoices { get; set; }

        public int TotalExportInvoices { get; set; }

        public int TotalAvailableGoods { get; set; }
    }
}
