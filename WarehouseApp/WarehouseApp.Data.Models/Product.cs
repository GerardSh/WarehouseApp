using Microsoft.EntityFrameworkCore;

namespace WarehouseApp.Data.Models
{
    [Comment("Products available in the warehouse")]
    public class Product
    {
        [Comment("Product identifier")]
        public Guid Id { get; set; }

        [Comment("Name of the product")]
        public required string Name { get; set; }

        [Comment("Description of the product, optional")]
        public string? Description { get; set; }

        [Comment("Foreign key to the Category")]
        public Guid CategoryId { get; set; }

        public virtual Category Category { get; set; } = null!;

        public virtual ICollection<ImportInvoiceDetail> ImportInvoicesDetails { get; set; }
            = new HashSet<ImportInvoiceDetail>();
    }
}
