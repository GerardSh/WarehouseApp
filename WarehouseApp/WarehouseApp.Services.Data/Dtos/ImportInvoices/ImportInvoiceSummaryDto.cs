namespace WarehouseApp.Services.Data.Dtos.ImportInvoices
{
    public class ImportInvoiceSummaryDto
    {
        public Guid Id { get; set; }

        public string InvoiceNumber { get; set; } = null!;

        public IEnumerable<Guid> ImportDetails
            = new List<Guid>();
    }
}