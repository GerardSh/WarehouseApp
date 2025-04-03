using WarehouseApp.Data.Models;
using static WarehouseApp.Common.Constants.EntityConstants.Category;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WarehouseApp.Data.Configuration
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> entity)
        {
            entity
                .HasKey(c => c.Id);

            entity
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            entity
                .Property(c => c.Description)
                .IsRequired(false)
                .HasMaxLength(DescriptionMaxLength);

            entity
                .HasData(SeedCategories());
        }

        private List<Category> SeedCategories()
        {
            return new List<Category>
            {
                new Category
                {
                    Id = Guid.Parse("31bf5b32-a3f7-4e37-8230-2202a081ea87"),
                    Name = "Computers & Peripherals",
                    Description = "Products related to computers and accessories such as keyboards, mice, and gaming gear."
                },
                new Category
                {
                    Id = Guid.Parse("3daadd5e-f5c9-4222-9878-1a02faa44339"),
                    Name = "Office Furniture & Accessories",
                    Description = "Furniture and accessories designed for improving office comfort and productivity."
                },
                new Category
                {
                    Id = Guid.Parse("e3edc37f-6ba9-4616-b0c5-98c33451bcdb"),
                    Name = "Home Appliances",
                    Description = "Small appliances for use at home such as coffee makers, speakers, and chargers."
                },
                new Category
                {
                    Id = Guid.Parse("314820d6-a4d4-4873-8dec-95e5a6904a8e"),
                    Name = "Wearable Tech",
                    Description = "Technology that you wear, including smartwatches and wireless earbuds."
                },
                new Category
                {
                    Id = Guid.Parse("dde109fb-71c5-4f59-8810-3bf4057c5351"),
                    Name = "Storage & Backup",
                    Description = "Storage solutions and backup devices for your data, such as external hard drives and SSDs."
                },
                new Category
                {
                    Id = Guid.Parse("26ea2ad2-8d2c-4c43-939f-d7d818572aff"),
                    Name = "Mobile Accessories",
                    Description = "Accessories to complement mobile devices, including power banks and docking stations."
                }
            };
        }
    }
}