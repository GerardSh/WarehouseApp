using Microsoft.EntityFrameworkCore;

namespace WarehouseApp.Data.Models
{
    [Comment("Warehouses in the system")]
    public class Warehouse
    {
        [Comment("Warehouse identifier")]
        public Guid Id { get; set; }

        [Comment("Warehouse name")]
        public required string Name { get; set; }

        [Comment("Warehouse address")]
        public required string Address { get; set; }

        [Comment("Warehouse size in square meters (m²)")]
        public double? Size { get; set; }

        [Comment("Shows the date of when the warehouse record was created")]
        public DateTime CreatedDate { get; set; }

        [Comment("Shows if warehouse is deleted")]
        public bool IsDeleted { get; set; }

        public virtual ICollection<ImportInvoice> ImportInvoices { get; set; }
            = new HashSet<ImportInvoice>();

        public virtual ICollection<ExportInvoice> ExportInvoices { get; set; }
            = new HashSet<ExportInvoice>();

        public virtual ICollection<ApplicationUserWarehouse> Users { get; set; }
            = new HashSet<ApplicationUserWarehouse>();
    }
}
