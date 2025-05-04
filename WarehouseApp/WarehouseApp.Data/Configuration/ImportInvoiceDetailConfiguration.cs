using WarehouseApp.Data.Models;
using static WarehouseApp.Common.Constants.EntityConstants;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WarehouseApp.Data.Configuration
{
    internal class ImportInvoiceDetailConfiguration : IEntityTypeConfiguration<ImportInvoiceDetail>
    {
        public void Configure(EntityTypeBuilder<ImportInvoiceDetail> entity)
        {
            entity
                .HasKey(iid => iid.Id);

            entity
                .HasOne(iid => iid.ImportInvoice)
                .WithMany(ii => ii.ImportInvoicesDetails)
                .HasForeignKey(iid => iid.ImportInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(iid => iid.Product)
                .WithMany(p => p.ImportInvoicesDetails)
                .HasForeignKey(iid => iid.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .Property(iid => iid.ImportInvoiceId)
                .IsRequired();

            entity
                .Property(iid => iid.ProductId)
                .IsRequired();

            entity
                .Property(iid => iid.Quantity)
                .IsRequired();

            entity
                .Property(iid => iid.UnitPrice)
                .IsRequired(false)
                .HasColumnType(MoneyType);

            entity
                .HasData(SeedImportInvoiceDetails());
        }

        private List<ImportInvoiceDetail> SeedImportInvoiceDetails()
        {
            return new List<ImportInvoiceDetail>
            {
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("5b53a1d4-7b7b-4127-bdb4-388c870e7082"),
                    ImportInvoiceId = Guid.Parse("4d3e97b5-119e-4cf2-8b78-4f5478a5a401"),
                    ProductId = Guid.Parse("a6a5c1b8-d5bc-46b5-b75c-0152e35e344e"),
                    Quantity = 10,
                    UnitPrice = 199.99m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("35f5e0b1-8e2e-4e2e-b46c-e1f392ff65a4"),
                    ImportInvoiceId = Guid.Parse("4d3e97b5-119e-4cf2-8b78-4f5478a5a401"),
                    ProductId = Guid.Parse("72c4bc6d-72f5-4b43-b299-cb1bcaa4cd93"),
                    Quantity = 5,
                    UnitPrice = 299.99m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("f44fa9ea-cf1b-4c1d-936e-b31f4d7298d3"),
                    ImportInvoiceId = Guid.Parse("d9784351-b2f6-44e5-ae5c-45615df7a102"),
                    ProductId = Guid.Parse("a4b3c7d5-9e1f-4f86-bc2d-0b5d4e3c7981"),
                    Quantity = 8,
                    UnitPrice = 149.50m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("c5b2f2fa-f5de-4172-b558-b8a0aa31cbd4"),
                    ImportInvoiceId = Guid.Parse("d9784351-b2f6-44e5-ae5c-45615df7a102"),
                    ProductId = Guid.Parse("a0b5d4c3-7e92-4f81-bc2d-5f3a7d1e4689"),
                    Quantity = 3,
                    UnitPrice = 89.99m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("d91fd94a-79f8-4023-9cc5-502a9ffacfd7"),
                    ImportInvoiceId = Guid.Parse("a1cfe4a7-dbc3-4c8f-9582-02efc6ea47f3"),
                    ProductId = Guid.Parse("ed2b3c7f-8a45-4d9e-80f4-b1c7d2a0f9b2"),
                    Quantity = 12,
                    UnitPrice = 59.99m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("cf13a1ec-99fc-4f7c-a635-45f35fc72ad5"),
                    ImportInvoiceId = Guid.Parse("b2d5cb4e-fc88-4c9f-9e9e-4a6a3f4e70d4"),
                    ProductId = Guid.Parse("d4c7e1a5-3b92-4f86-92d1-0b5f3a7c4682"),
                    Quantity = 15,
                    UnitPrice = 210.00m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("7a1bfb82-7a6e-47f9-bbbf-81093e4a2876"),
                    ImportInvoiceId = Guid.Parse("b2d5cb4e-fc88-4c9f-9e9e-4a6a3f4e70d4"),
                    ProductId = Guid.Parse("72c4bc6d-72f5-4b43-b299-cb1bcaa4cd93"),
                    Quantity = 7,
                    UnitPrice = 320.00m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("ebef2229-41c0-41fa-a56c-f3a9d7618ad4"),
                    ImportInvoiceId = Guid.Parse("b2d5cb4e-fc88-4c9f-9e9e-4a6a3f4e70d4"),
                    ProductId = Guid.Parse("f1a5b3c7-74d9-4e8f-92b1-c3d2a0b5f8e6"),
                    Quantity = 20,
                    UnitPrice = 140.00m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("328cc44d-d73f-4956-b2cb-dc7cd1c4f437"),
                    ImportInvoiceId = Guid.Parse("b2d5cb4e-fc88-4c9f-9e9e-4a6a3f4e70d4"),
                    ProductId = Guid.Parse("a0b5d4c3-7e92-4f81-bc2d-5f3a7d1e4689"),
                    Quantity = 10,
                    UnitPrice = 95.00m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("4532b6e6-53cf-44a1-9392-8a85f2c89901"),
                    ImportInvoiceId = Guid.Parse("b2d5cb4e-fc88-4c9f-9e9e-4a6a3f4e70d4"),
                    ProductId = Guid.Parse("a5b3d7c2-9e1f-4f86-bc2d-0b5d4c3e7981"),
                    Quantity = 6,
                    UnitPrice = 65.00m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("b0a77ed1-4c9b-4056-a05c-f01e6be521f0"),
                    ImportInvoiceId = Guid.Parse("8e9d59b2-f682-4e4b-8230-538eef5772dc"),
                    ProductId = Guid.Parse("72c4bc6d-72f5-4b43-b299-cb1bcaa4cd93"),
                    Quantity = 4,
                    UnitPrice = 310.00m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("39e3849a-b72d-4372-94f6-7e42f7c3cb35"),
                    ImportInvoiceId = Guid.Parse("8e9d59b2-f682-4e4b-8230-538eef5772dc"),
                    ProductId = Guid.Parse("d4c7e1a5-3b92-4f86-92d1-0b5f3a7c4682"),
                    Quantity = 9,
                    UnitPrice = 90.00m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("278b0a44-5c4e-42ff-a80f-dcc4b9acb306"),
                    ImportInvoiceId = Guid.Parse("fa61f3b4-e46f-496e-b90f-bd470f5b68ac"),
                    ProductId = Guid.Parse("b7c4d5e2-3a92-4f86-91b1-7d3a0b5e4683"),
                    Quantity = 4,
                    UnitPrice = 120.00m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("1a396e47-1ff1-4f8b-b8c9-f3b82b3a8c83"),
                    ImportInvoiceId = Guid.Parse("e50e7e92-b28f-47f7-99f3-ff442f234cb2"),
                    ProductId = Guid.Parse("f1a5b3c7-74d9-4e8f-92b1-c3d2a0b5f8e6"),
                    Quantity = 15,
                    UnitPrice = 149.99m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("d1a60174-118f-46b5-91a2-16a55e6a0b44"),
                    ImportInvoiceId = Guid.Parse("e50e7e92-b28f-47f7-99f3-ff442f234cb2"),
                    ProductId = Guid.Parse("e7a2c5d4-3b9f-4871-82c1-d5b4a0f6e398"),
                    Quantity = 8,
                    UnitPrice = 119.50m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("cbf676be-59d0-4db7-b0ec-3cd7d32f2b73"),
                    ImportInvoiceId = Guid.Parse("e50e7e92-b28f-47f7-99f3-ff442f234cb2"),
                    ProductId = Guid.Parse("b4c9d2e7-1a3f-4856-bc91-f5d3a0b7e462"),
                    Quantity = 10,
                    UnitPrice = 199.99m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("4b31e9db-d573-4d58-bd9f-0df688f5cd60"),
                    ImportInvoiceId = Guid.Parse("e50e7e92-b28f-47f7-99f3-ff442f234cb2"),
                    ProductId = Guid.Parse("c8f3a7d2-1b6e-4c5d-92f4-a0b5d4e7c9f1"),
                    Quantity = 5,
                    UnitPrice = 259.99m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("3833b38d-828d-47ae-90bc-7fe2f21cb0cc"),
                    ImportInvoiceId = Guid.Parse("e50e7e92-b28f-47f7-99f3-ff442f234cb2"),
                    ProductId = Guid.Parse("a6a5c1b8-d5bc-46b5-b75c-0152e35e344e"),
                    Quantity = 20,
                    UnitPrice = 349.99m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("b319cb60-043f-462f-8a06-6a17b66de6ea"),
                    ImportInvoiceId = Guid.Parse("e50e7e92-b28f-47f7-99f3-ff442f234cb2"),
                    ProductId = Guid.Parse("d4c7e1a5-3b92-4f86-92d1-0b5f3a7c4682"),
                    Quantity = 12,
                    UnitPrice = 449.99m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("bb93b12f-c8b1-4d2e-a53b-1e05f0e6fe0a"),
                    ImportInvoiceId = Guid.Parse("f8ed3bb4-d255-4928-8a29-c24e045f302d"),
                    ProductId = Guid.Parse("72c4bc6d-72f5-4b43-b299-cb1bcaa4cd93"),
                    Quantity = 3,
                    UnitPrice = 289.99m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("ed547c62-e8d2-413f-9c7d-cfffc8a9da6c"),
                    ImportInvoiceId = Guid.Parse("f8ed3bb4-d255-4928-8a29-c24e045f302d"),
                    ProductId = Guid.Parse("c3a9d582-74b6-4512-91a5-2643a1b682be"),
                    Quantity = 10,
                    UnitPrice = 159.99m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("b3c5a697-dba0-4b5e-bb61-7f2c455e7ac1"),
                    ImportInvoiceId = Guid.Parse("a38f5c8b-c8d6-4590-bf5f-f5d616f1c1b7"),
                    ProductId = Guid.Parse("a6a5c1b8-d5bc-46b5-b75c-0152e35e344e"),
                    Quantity = 20,
                    UnitPrice = 109.99m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("7c2e50c5-28ba-4234-b1ae-e1d70962b4ea"),
                    ImportInvoiceId = Guid.Parse("c4f631f3-16a1-47a4-8987-5a4b9a24b4d6"),
                    ProductId = Guid.Parse("f1a3b5c7-92d4-4f86-8b1d-0b5e4c3d7982"),
                    Quantity = 15,
                    UnitPrice = 59.99m
                },
                new ImportInvoiceDetail
                {
                    Id = Guid.Parse("3bafbb58-56e0-4bba-9ab5-d70832ff56f4"),
                    ImportInvoiceId = Guid.Parse("c4f631f3-16a1-47a4-8987-5a4b9a24b4d6"),
                    ProductId = Guid.Parse("72c4bc6d-72f5-4b43-b299-cb1bcaa4cd93"),
                    Quantity = 8,
                    UnitPrice = 299.99m
                }
            };
        }
    }
}
