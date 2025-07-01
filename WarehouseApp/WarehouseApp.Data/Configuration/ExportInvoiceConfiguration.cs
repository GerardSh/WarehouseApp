using WarehouseApp.Data.Models;
using static WarehouseApp.Common.Constants.EntityConstants.ExportInvoice;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WarehouseApp.Data.Configuration
{
    internal class ExportInvoiceConfiguration : IEntityTypeConfiguration<ExportInvoice>
    {
        public void Configure(EntityTypeBuilder<ExportInvoice> entity)
        {
            entity
                .HasKey(ei => ei.Id);

            entity
                .Property(ei => ei.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(InvoiceNumberMaxLength);

            entity
                .Property(ei => ei.ClientId)
                .IsRequired();

            entity
                .HasOne(ei => ei.Client)
                .WithMany(c => c.ExportInvoices)
                .HasForeignKey(ei => ei.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .Property(ii => ii.WarehouseId)
                .IsRequired();

            entity
                .HasOne(ei => ei.Warehouse)
                .WithMany(w => w.ExportInvoices)
                .HasForeignKey(ei => ei.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction);

            entity
               .HasIndex(ii => new { ii.WarehouseId, ii.InvoiceNumber })
               .IsUnique();

            entity
                .HasData(SeedExportInvoices());
        }

        private List<ExportInvoice> SeedExportInvoices()
        {
            return new List<ExportInvoice>
            {
                new ExportInvoice
                {
                    Id = Guid.Parse("f4c7e1a5-4392-4f86-92d1-0b5f3a7c46f2"),
                    InvoiceNumber = "EXP001",
                    Date = new DateTime(2025, 4, 16),
                    ClientId = Guid.Parse("8a3f9b62-1e24-46f1-bc95-71f25a4c0e9a"),
                    WarehouseId = Guid.Parse("b689e5b1-8c23-462d-b931-97a7d2b40470")
                },
                new ExportInvoice
                {
                    Id = Guid.Parse("a1d1c8f3-91b0-4f62-b7b1-b1d13a7e75c8"),
                    InvoiceNumber = "EXP002",
                    Date = new DateTime(2025, 4, 17),
                    ClientId = Guid.Parse("8a3f9b62-1e24-46f1-bc95-71f25a4c0e9a"),
                    WarehouseId = Guid.Parse("b689e5b1-8c23-462d-b931-97a7d2b40470")
                },
                new ExportInvoice
                {
                    Id = Guid.Parse("4db5f5f4-8be3-4ecf-98cd-6d4a4f1d2cf2"),
                    InvoiceNumber = "EXP003",
                    Date = new DateTime(2025, 4, 19),
                    ClientId = Guid.Parse("8a3f9b62-1e24-46f1-bc95-71f25a4c0e9a"),
                    WarehouseId = Guid.Parse("b689e5b1-8c23-462d-b931-97a7d2b40470")
                },
                new ExportInvoice
                {
                    Id = Guid.Parse("8b5c1e28-1bc7-4c70-bb2e-2f2c8e5b39f5"),
                    InvoiceNumber = "EXP004",
                    Date = new DateTime(2025, 4, 23),
                    ClientId = Guid.Parse("f56d3c8b-92a4-4d71-82c7-6f4b9e2c1f0d"),
                    WarehouseId = Guid.Parse("b689e5b1-8c23-462d-b931-97a7d2b40470")
                },
                new ExportInvoice
                {
                    Id = Guid.Parse("b65c7e3a-76a3-42b3-bbc2-73c1a693743e"),
                    InvoiceNumber = "EXP005",
                    Date = new DateTime(2025, 4, 25),
                    ClientId = Guid.Parse("b2d9f1c5-84e7-4c1b-92a6-3a5e7c0f8b29"),
                    WarehouseId = Guid.Parse("b689e5b1-8c23-462d-b931-97a7d2b40470")
                },
                new ExportInvoice
                {
                    Id = Guid.Parse("fe7f4c16-f76f-42e9-bbfe-6c9e6b87e33b"),
                    InvoiceNumber = "EXP006",
                    Date = new DateTime(2025, 4, 26),
                    ClientId = Guid.Parse("f8c3a5d7-21b4-4e9c-86f2-7d5b3a9e1c4b"),
                    WarehouseId = Guid.Parse("b689e5b1-8c23-462d-b931-97a7d2b40470")
                },
                new ExportInvoice
                {
                    Id = Guid.Parse("dcb7f1ae-b8f1-4670-93b2-5e7ff7d3c3c6"),
                    InvoiceNumber = "EXP007",
                    Date = new DateTime(2025, 4, 29),
                    ClientId = Guid.Parse("d9f1b25c-84e6-4c1a-92b5-3a5e8c0f7b19"),
                    WarehouseId = Guid.Parse("b689e5b1-8c23-462d-b931-97a7d2b40470")
                },
                new ExportInvoice
                {
                    Id = Guid.Parse("3f36e42f-799b-4d60-a96f-3f07947a66d0"),
                    InvoiceNumber = "EXP008",
                    Date = new DateTime(2025, 4, 30),
                    ClientId = Guid.Parse("e3b0c442-98fc-1c14-9afb-f4c8996fb924"),
                    WarehouseId = Guid.Parse("b689e5b1-8c23-462d-b931-97a7d2b40470")
                },
                new ExportInvoice
                {
                    Id = Guid.Parse("bc107ca4-2833-4f26-9d1e-36ab9080c418"),
                    InvoiceNumber = "EXP009",
                    Date = new DateTime(2025, 5, 1),
                    ClientId = Guid.Parse("f56d3c8b-92a4-4d71-82c7-6f4b9e2c1f0d"),
                    WarehouseId = Guid.Parse("b689e5b1-8c23-462d-b931-97a7d2b40470")
                },
                new ExportInvoice
                {
                    Id = Guid.Parse("cae789ba-1bcd-4e32-b57c-8243fc4d8d19"),
                    InvoiceNumber = "EXP010",
                    Date = new DateTime(2025, 5, 4),
                    ClientId = Guid.Parse("d9f1b25c-84e6-4c1a-92b5-3a5e8c0f7b19"),
                    WarehouseId = Guid.Parse("b689e5b1-8c23-462d-b931-97a7d2b40470")
                }
            };
        }
    }
}
