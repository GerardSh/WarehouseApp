using WarehouseApp.Web.ViewModels.Warehouse;

namespace WarehouseApp.Web.ViewModels.Shared
{
    public class AllWarehousesSearchFilterViewModel
    {
        public IEnumerable<WarehouseCardViewModel> Warehouses { get; set; }
                = new List<WarehouseCardViewModel>();

        public string? SearchQuery { get; set; }

        public string? YearFilter { get; set; }

        public int? CurrentPage { get; set; } = 1;

        public int? EntitiesPerPage { get; set; } = 5;

        public int TotalItemsBeforePagination { get; set; }

        public int? TotalPages { get; set; }

        public int TotalUserWarehouses { get; set; }
    }
}
