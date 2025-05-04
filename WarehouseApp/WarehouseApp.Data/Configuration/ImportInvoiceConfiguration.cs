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

            entity
                .HasData(SeedImportInvoices());
        }

        private List<ImportInvoice> SeedImportInvoices()
        {
            return new List<ImportInvoice>
            {
                new ImportInvoice
                {
                    Id = Guid.Parse("4d3e97b5-119e-4cf2-8b78-4f5478a5a401"),
                    InvoiceNumber = "INV001",
                    Date = new DateTime(2025, 4, 2),
                    SupplierId = Guid.Parse("e3b0c442-98fc-1c14-9afb-f4c8996fb924"),
                    WarehouseId = Guid.Parse("b689e5b1-8c23-462d-b931-97a7d2b40470"),
                },          
                new ImportInvoice
                {
                    Id = Guid.Parse("d9784351-b2f6-44e5-ae5c-45615df7a102"),
                    InvoiceNumber = "INV002",
                    Date = new DateTime(2025, 4, 10),
                    SupplierId = Guid.Parse("3b12c292-b084-491b-9b3d-3e01b7ff2eaf"),
                    WarehouseId = Guid.Parse("b689e5b1-8c23-462d-b931-97a7d2b40470"),
                },     
                new ImportInvoice
                {
                    Id = Guid.Parse("a1cfe4a7-dbc3-4c8f-9582-02efc6ea47f3"),
                    InvoiceNumber = "INV003",
                    Date = new DateTime(2025, 4, 11),
                    SupplierId = Guid.Parse("f56d3c8b-92a4-4d71-82c7-6f4b9e2c1f0d"),
                    WarehouseId = Guid.Parse("b689e5b1-8c23-462d-b931-97a7d2b40470"),
                },
                new ImportInvoice
                {
                    Id = Guid.Parse("b2d5cb4e-fc88-4c9f-9e9e-4a6a3f4e70d4"),
                    InvoiceNumber = "INV004",
                    Date = new DateTime(2025, 4, 13),
                    SupplierId = Guid.Parse("a1c4b9e5-7f28-48c1-b62d-4f3a5d7e92b8"),
                    WarehouseId = Guid.Parse("b689e5b1-8c23-462d-b931-97a7d2b40470"),
                },
                new ImportInvoice
                {
                    Id = Guid.Parse("8e9d59b2-f682-4e4b-8230-538eef5772dc"),
                    InvoiceNumber = "INV005",
                    Date = new DateTime(2025, 4, 15),
                    SupplierId = Guid.Parse("b2d9f1c5-84e7-4c1b-92a6-3a5e7c0f8b29"),
                    WarehouseId = Guid.Parse("b689e5b1-8c23-462d-b931-97a7d2b40470"),
                },
                new ImportInvoice
                {
                    Id = Guid.Parse("fa61f3b4-e46f-496e-b90f-bd470f5b68ac"),
                    InvoiceNumber = "INV006",
                    Date = new DateTime(2025, 4, 20),
                    SupplierId = Guid.Parse("8a3f9b62-1e24-46f1-bc95-71f25a4c0e9a"),
                    WarehouseId = Guid.Parse("be8f00a5-682d-4b43-9734-d3e17078cb52"),
                },
                new ImportInvoice
                {
                    Id = Guid.Parse("e50e7e92-b28f-47f7-99f3-ff442f234cb2"),
                    InvoiceNumber = "INV007",
                    Date = new DateTime(2025, 4, 25),
                    SupplierId = Guid.Parse("e3b0c442-98fc-1c14-9afb-f4c8996fb924"),
                    WarehouseId = Guid.Parse("be8f00a5-682d-4b43-9734-d3e17078cb52"),
                },
                new ImportInvoice
                {
                    Id = Guid.Parse("f8ed3bb4-d255-4928-8a29-c24e045f302d"),
                    InvoiceNumber = "INV008",
                    Date = new DateTime(2025, 5, 1),
                    SupplierId = Guid.Parse("b3e2c5a1-7d84-4f6b-91c2-5a7f3b8e9d4f"),
                    WarehouseId = Guid.Parse("be8f00a5-682d-4b43-9734-d3e17078cb52"),
                },
                new ImportInvoice
                {
                    Id = Guid.Parse("a38f5c8b-c8d6-4590-bf5f-f5d616f1c1b7"),
                    InvoiceNumber = "INV009",
                    Date = new DateTime(2025, 5, 2),
                    SupplierId = Guid.Parse("f56d3c8b-92a4-4d71-82c7-6f4b9e2c1f0d"),
                    WarehouseId = Guid.Parse("be8f00a5-682d-4b43-9734-d3e17078cb52"),
                },
                new ImportInvoice
                {
                    Id = Guid.Parse("c4f631f3-16a1-47a4-8987-5a4b9a24b4d6"),
                    InvoiceNumber = "INV010",
                    Date = new DateTime(2025, 5, 4),
                    SupplierId = Guid.Parse("a1c4b9e5-7f28-48c1-b62d-4f3a5d7e92b8"),
                    WarehouseId = Guid.Parse("be8f00a5-682d-4b43-9734-d3e17078cb52"),
                }
            };
        }
    }
}
