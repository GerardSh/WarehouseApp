using WarehouseApp.Data.Models;
using static WarehouseApp.Common.Constants.EntityConstants.Warehouse;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WarehouseApp.Data.Configuration
{
    internal class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> entity)
        {
            entity
                .HasKey(w => w.Id);

            entity
                .Property(w => w.Size)
                .IsRequired(false);

            entity
                .Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            entity
                .Property(w => w.Address)
                .IsRequired()
                .HasMaxLength(AddressMaxLength);

            entity
                .Property(w => w.CreatedDate)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("GETUTCDATE()");

            entity
                .Property(w => w.CreatedByUserId)
                .IsRequired(false);

            entity
                .HasIndex(w => new { w.Name, w.CreatedByUserId })
                .IsUnique();

            entity
                .HasOne(w => w.CreatedByUser)
                .WithMany()
                .HasForeignKey(w => w.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasQueryFilter(w => !w.IsDeleted);

            entity
                .HasData(SeedWarehouses());
        }

        private List<Warehouse> SeedWarehouses()
        {
            return new List<Warehouse>
            {
                new Warehouse
                {
                    Id = Guid.Parse("b689e5b1-8c23-462d-b931-97a7d2b40470"),
                    Name = "Warehouse A",
                    Address = "Location A",
                    Size = 4650,
                    CreatedDate = DateTime.MinValue
                },
                new Warehouse
                {
                    Id = Guid.Parse("be8f00a5-682d-4b43-9734-d3e17078cb52"),
                    Name = "Warehouse B",
                    Address = "Location B",
                    Size = 5200,
                    CreatedDate = DateTime.MinValue
                },
                new Warehouse
                {
                    Id = Guid.Parse("c3e1a7d3-8e44-4f9b-bf8b-1d3f6e7f8d42"),
                    Name = "Warehouse C",
                    Address = "Location C",
                    Size = 4500,
                    CreatedDate = DateTime.MinValue
                },
                new Warehouse
                {
                    Id = Guid.Parse("d5f6b4e8-3b67-4a7d-bc12-f4a9e6c8d351"),
                    Name = "Warehouse D",
                    Address = "Location D",
                    Size = 6300,
                    CreatedDate = DateTime.MinValue
                },
                new Warehouse
                {
                    Id = Guid.Parse("e8a9c5b7-4d23-49fb-a91b-c6e1f2d8b643"),
                    Name = "Warehouse E",
                    Address = "Location E",
                    Size = 7100,
                    CreatedDate = DateTime.MinValue
                },
                new Warehouse
                {
                    Id = Guid.Parse("f1c2d4a6-5b34-42d8-9c12-e3a7b8f6d921"),
                    Name = "Warehouse F",
                    Address = "Location F",
                    Size = 5400,
                    CreatedDate = DateTime.MinValue
                },
                new Warehouse
                {
                    Id = Guid.Parse("a2b3c4d5-6e78-4f9a-bc12-d4e5f6a7b891"),
                    Name = "Warehouse G",
                    Address = "Location G",
                    Size = 5900,
                    CreatedDate = DateTime.MinValue
                },
                new Warehouse
                {
                    Id = Guid.Parse("b4c5d6e7-8f91-42a3-bc12-e3f4a5d6b782"),
                    Name = "Warehouse H",
                    Address = "Location H",
                    Size = 4800,
                    CreatedDate = DateTime.MinValue
                },
                new Warehouse
                {
                    Id = Guid.Parse("c6d7e8f9-1a23-45b4-bc12-d3e4f5a6b891"),
                    Name = "Warehouse I",
                    Address = "Location I",
                    Size = 5200,
                    CreatedDate = DateTime.MinValue
                },
                new Warehouse
                {
                    Id = Guid.Parse("d8e9f1a2-3b45-47c6-bc12-e2f3a4d5b691"),
                    Name = "Warehouse J",
                    Address = "Location J",
                    Size = 6000,
                    CreatedDate = DateTime.MinValue
                }
            };
        }
    }
}
