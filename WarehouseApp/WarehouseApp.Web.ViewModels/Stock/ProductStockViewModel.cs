namespace WarehouseApp.Web.ViewModels.Stock
{
    public class ProductStockViewModel
    {
        public string ProductName { get; set; } = null!;

        public string CategoryName { get; set; } = null!;

        public int TotalImported { get; set; }

        public int TotalExported { get; set; }

        public int Available => TotalImported - TotalExported;
    }
}
