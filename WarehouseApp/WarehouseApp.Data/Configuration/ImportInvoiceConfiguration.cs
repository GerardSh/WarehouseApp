using WarehouseApp.Data.Models;
using static WarehouseApp.Common.Constants.EntityConstants.ImportInvoice;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WarehouseApp.Data.Configuration
{
    internal class ImportInvoiceConfiguration : IEntityTypeConfiguration<ImportInvoice>
    {
        public void Configure(EntityTypeBuilder<ImportInvoice> entity)
        {
            entity
                .HasKey(ii => ii.Id);

            entity
                .Property(ii => ii.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(InvoiceNumberMaxLength);

            entity
                .Property(ii => ii.SupplierId)
                .IsRequired();

            entity
                .HasOne(ii => ii.Supplier)
                .WithMany(c => c.ImportInvoices)
                .HasForeignKey(ii => ii.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .Property(ii => ii.WarehouseId)
                .IsRequired();

            entity
                .HasOne(ii => ii.Warehouse)
                .WithMany(w => w.ImportInvoices)
                .HasForeignKey(ii => ii.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction);

            entity
               .HasIndex(ii => new { ii.WarehouseId, ii.InvoiceNumber })
               .IsUnique();
        }
    }
}
