using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WarehouseApp.Data.Models;

namespace WarehouseApp.Data.Configuration
{
    public class ApplicationUserWarehouseConfiguration : IEntityTypeConfiguration<ApplicationUserWarehouse>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserWarehouse> entity)
        {
            entity
                .HasKey(uw => new { uw.ApplicationUserId, uw.WarehouseId });

            entity
                .HasOne(uw => uw.ApplicationUser)
                .WithMany(u => u.UserWarehouses)
                .HasForeignKey(uw => uw.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(uw => uw.Warehouse)
                .WithMany(w => w.WarehouseUsers)
                .HasForeignKey(uw => uw.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasQueryFilter(uw => !uw.Warehouse.IsDeleted);
        }
    }
}
