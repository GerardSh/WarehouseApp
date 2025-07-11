namespace WarehouseApp.Web.ViewModels.Warehouse
{
    public class WarehouseCardViewModel
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string CreatedDate { get; set; } = null!;

        public string? Size { get; set; }
    }
}
