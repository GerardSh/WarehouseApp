namespace WarehouseApp.Web.ViewModels.Stock
{
    public class AllProductsSearchFilterViewModel
    {
        public Guid WarehouseId { get; set; }

        public string WarehouseName { get; set; } = null!;

        public string? ProductQuery { get; set; }

        public string? CategoryQuery { get; set; }

        public bool IncludeExportedProducts { get; set; }

        public int? CurrentPage { get; set; } = 1;

        public int? EntitiesPerPage { get; set; } = 5;

        public int TotalItemsBeforePagination { get; set; }

        public int? TotalPages { get; set; }

        public int TotalProducts { get; set; }

        public IEnumerable<ProductStockViewModel> Products { get; set; }
        = new List<ProductStockViewModel>();
    }
}
