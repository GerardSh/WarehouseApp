using WarehouseApp.Data.Models;
using static WarehouseApp.Common.Constants.EntityConstants;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WarehouseApp.Data.Configuration
{
    internal class ExportInvoiceDetailConfiguration : IEntityTypeConfiguration<ExportInvoiceDetail>
    {
        public void Configure(EntityTypeBuilder<ExportInvoiceDetail> entity)
        {
            entity
                .HasKey(eid => eid.Id);

            entity
                .HasIndex(e => new { e.ExportInvoiceId, e.ImportInvoiceDetailId })
                .IsUnique();

            entity
                .HasOne(eid => eid.ExportInvoice)
                .WithMany(ei => ei.ExportInvoicesDetails)
                .HasForeignKey(eid => eid.ExportInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(eid => eid.ImportInvoiceDetail)
                .WithMany(p => p.ExportInvoicesPerProduct)
                .HasForeignKey(eid => eid.ImportInvoiceDetailId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .Property(eid => eid.ExportInvoiceId)
                .IsRequired();

            entity
                .Property(eid => eid.ImportInvoiceDetailId)
                .IsRequired();

            entity
                .Property(eid => eid.Quantity)
                .IsRequired();

            entity
                .Property(eid => eid.UnitPrice)
                .IsRequired(false)
                .HasColumnType(MoneyType);

            entity
                .HasData(SeedExportInvoiceDetails());
        }

        private List<ExportInvoiceDetail> SeedExportInvoiceDetails()
        {
            return new List<ExportInvoiceDetail>
            {
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("cc98d9a1-6e55-4b1e-99a4-1e1f36b0c1c1"),
                    ExportInvoiceId = Guid.Parse("f4c7e1a5-4392-4f86-92d1-0b5f3a7c46f2"),
                    ImportInvoiceDetailId = Guid.Parse("5b53a1d4-7b7b-4127-bdb4-388c870e7082"),
                    Quantity = 5,
                    UnitPrice = 209.99m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("b73f4d10-f2a9-4983-a96b-95ff3f3a67f0"),
                    ExportInvoiceId = Guid.Parse("f4c7e1a5-4392-4f86-92d1-0b5f3a7c46f2"),
                    ImportInvoiceDetailId = Guid.Parse("35f5e0b1-8e2e-4e2e-b46c-e1f392ff65a4"),
                    Quantity = 5,
                    UnitPrice = 309.99m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("d09c3c46-f247-4a7e-a7b0-0ab7b2cf8039"),
                    ExportInvoiceId = Guid.Parse("a1d1c8f3-91b0-4f62-b7b1-b1d13a7e75c8"),
                    ImportInvoiceDetailId = Guid.Parse("5b53a1d4-7b7b-4127-bdb4-388c870e7082"),
                    Quantity = 5,
                    UnitPrice = 209.99m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("e6a2b1f9-6df5-4d0e-9877-137b191c6b80"),
                    ExportInvoiceId = Guid.Parse("a1d1c8f3-91b0-4f62-b7b1-b1d13a7e75c8"),
                    ImportInvoiceDetailId = Guid.Parse("d91fd94a-79f8-4023-9cc5-502a9ffacfd7"),
                    Quantity = 12,
                    UnitPrice = 69.99m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("a059a5c3-e759-40a1-8d02-b1a3f8c04dc2"),
                    ExportInvoiceId = Guid.Parse("a1d1c8f3-91b0-4f62-b7b1-b1d13a7e75c8"),
                    ImportInvoiceDetailId = Guid.Parse("c5b2f2fa-f5de-4172-b558-b8a0aa31cbd4"),
                    Quantity = 2,
                    UnitPrice = 99.99m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("06b2a659-5a9c-4ad3-a324-55b2b06d7f27"),
                    ExportInvoiceId = Guid.Parse("4db5f5f4-8be3-4ecf-98cd-6d4a4f1d2cf2"),
                    ImportInvoiceDetailId = Guid.Parse("c5b2f2fa-f5de-4172-b558-b8a0aa31cbd4"),
                    Quantity = 1,
                    UnitPrice = 99.99m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("f86c942d-c6c0-4cc5-8b33-768647f3e96e"),
                    ExportInvoiceId = Guid.Parse("4db5f5f4-8be3-4ecf-98cd-6d4a4f1d2cf2"),
                    ImportInvoiceDetailId = Guid.Parse("cf13a1ec-99fc-4f7c-a635-45f35fc72ad5"),
                    Quantity = 10,
                    UnitPrice = 220.00m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("46c8c119-faf9-4e3c-8be1-0f3de6c287d4"),
                    ExportInvoiceId = Guid.Parse("4db5f5f4-8be3-4ecf-98cd-6d4a4f1d2cf2"),
                    ImportInvoiceDetailId = Guid.Parse("39e3849a-b72d-4372-94f6-7e42f7c3cb35"),
                    Quantity = 3,
                    UnitPrice =100.00m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("e2517aef-6fd1-4d08-9a20-c632582d5b32"),
                    ExportInvoiceId = Guid.Parse("4db5f5f4-8be3-4ecf-98cd-6d4a4f1d2cf2"),
                    ImportInvoiceDetailId = Guid.Parse("b0a77ed1-4c9b-4056-a05c-f01e6be521f0"),
                    Quantity = 4,
                    UnitPrice = 320.00m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("59e1bd10-c198-4b30-b5e4-0fc97a77d8c1"),
                    ExportInvoiceId = Guid.Parse("8b5c1e28-1bc7-4c70-bb2e-2f2c8e5b39f5"),
                    ImportInvoiceDetailId = Guid.Parse("ebef2229-41c0-41fa-a56c-f3a9d7618ad4"),
                    Quantity = 12,
                    UnitPrice = 150.00m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("991cbf9e-c58e-4d13-9988-7ddf3de1d91e"),
                    ExportInvoiceId = Guid.Parse("b65c7e3a-76a3-42b3-bbc2-73c1a693743e"),
                    ImportInvoiceDetailId = Guid.Parse("cf13a1ec-99fc-4f7c-a635-45f35fc72ad5"),
                    Quantity = 3,
                    UnitPrice = 220.00m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("87a64d60-e444-4b84-b88f-9d5c78d4fc72"),
                    ExportInvoiceId = Guid.Parse("b65c7e3a-76a3-42b3-bbc2-73c1a693743e"),
                    ImportInvoiceDetailId = Guid.Parse("f44fa9ea-cf1b-4c1d-936e-b31f4d7298d3"),
                    Quantity = 3,
                    UnitPrice = 159.50m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("cb884fc2-b770-4632-9a10-29b4b5e16e64"),
                    ExportInvoiceId = Guid.Parse("fe7f4c16-f76f-42e9-bbfe-6c9e6b87e33b"),
                    ImportInvoiceDetailId = Guid.Parse("7a1bfb82-7a6e-47f9-bbbf-81093e4a2876"),
                    Quantity = 4,
                    UnitPrice = 330.00m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("bc09320d-4e82-4b8b-89a6-f9eb69a401d6"),
                    ExportInvoiceId = Guid.Parse("fe7f4c16-f76f-42e9-bbfe-6c9e6b87e33b"),
                    ImportInvoiceDetailId = Guid.Parse("ebef2229-41c0-41fa-a56c-f3a9d7618ad4"),
                    Quantity = 2,
                    UnitPrice = 150.00m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("4e0d9c60-1051-4a0e-a676-bd2a3a24b1cf"),
                    ExportInvoiceId = Guid.Parse("dcb7f1ae-b8f1-4670-93b2-5e7ff7d3c3c6"),
                    ImportInvoiceDetailId = Guid.Parse("328cc44d-d73f-4956-b2cb-dc7cd1c4f437"),
                    Quantity = 10,
                    UnitPrice = 105.00m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("763b9cb0-d02d-468b-83ef-1f245b599d93"),
                    ExportInvoiceId = Guid.Parse("3f36e42f-799b-4d60-a96f-3f07947a66d0"),
                    ImportInvoiceDetailId = Guid.Parse("39e3849a-b72d-4372-94f6-7e42f7c3cb35"),
                    Quantity = 3,
                    UnitPrice = 100.00m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("a0f0e6bd-8a18-4be8-931b-bd2928b8bbd7"),
                    ExportInvoiceId = Guid.Parse("bc107ca4-2833-4f26-9d1e-36ab9080c418"),
                    ImportInvoiceDetailId = Guid.Parse("cf13a1ec-99fc-4f7c-a635-45f35fc72ad5"),
                    Quantity = 1,
                    UnitPrice = 220.00m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("92e4e83c-2b56-40d0-aefd-f99d9ff7030b"),
                    ExportInvoiceId = Guid.Parse("bc107ca4-2833-4f26-9d1e-36ab9080c418"),
                    ImportInvoiceDetailId = Guid.Parse("ebef2229-41c0-41fa-a56c-f3a9d7618ad4"),
                    Quantity = 3,
                    UnitPrice = 150.0m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("cfcb0e87-8be3-44ef-bad6-417dc560e0e6"),
                    ExportInvoiceId = Guid.Parse("bc107ca4-2833-4f26-9d1e-36ab9080c418"),
                    ImportInvoiceDetailId = Guid.Parse("f44fa9ea-cf1b-4c1d-936e-b31f4d7298d3"),
                    Quantity = 2,
                    UnitPrice = 159.5m
                },
                new ExportInvoiceDetail
                {
                    Id = Guid.Parse("3f218f66-7b42-429c-bb02-5cd9b7d1147b"),
                    ExportInvoiceId = Guid.Parse("cae789ba-1bcd-4e32-b57c-8243fc4d8d19"),
                    ImportInvoiceDetailId = Guid.Parse("ebef2229-41c0-41fa-a56c-f3a9d7618ad4"),
                    Quantity = 2,
                    UnitPrice = 150.0m
                }
            };
        }
    }
}
