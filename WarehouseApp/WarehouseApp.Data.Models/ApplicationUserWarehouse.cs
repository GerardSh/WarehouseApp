using Microsoft.EntityFrameworkCore;

namespace WarehouseApp.Data.Models
{
    [Comment("Mapping table between application users and warehouses")]
    public class ApplicationUserWarehouse
    {
        [Comment("Foreign key to the ApplicationUser")]
        public Guid ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        public Guid WarehouseId { get; set; }

        [Comment("Foreign key to the Warehouse")]
        public virtual Warehouse Warehouse { get; set; } = null!;
    }
}
