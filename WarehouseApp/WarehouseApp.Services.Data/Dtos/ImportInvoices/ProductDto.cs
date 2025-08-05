namespace WarehouseApp.Services.Data.Dtos.ImportInvoices
{
    public class ProductDto
    {
        public string Name { get; set; } = null!;

        public string Category { get; set; } = null!;

        public int AvailableQuantity { get; set; }
    }
}
