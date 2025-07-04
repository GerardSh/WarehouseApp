﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
                .WithMany(u => u.Warehouses)
                .HasForeignKey(uw => uw.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(aw => aw.Warehouse)
                .WithMany(w => w.Users)
                .HasForeignKey(aw => aw.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
