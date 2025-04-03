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
                    Id = Guid.NewGuid(),
                    Name = "Computers & Peripherals",
                    Description = "Products related to computers and accessories such as keyboards, mice, and gaming gear."
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Office Furniture & Accessories",
                    Description = "Furniture and accessories designed for improving office comfort and productivity."
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Home Appliances",
                    Description = "Small appliances for use at home such as coffee makers, speakers, and chargers."
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Wearable Tech",
                    Description = "Technology that you wear, including smartwatches and wireless earbuds."
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Storage & Backup",
                    Description = "Storage solutions and backup devices for your data, such as external hard drives and SSDs."
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Mobile Accessories",
                    Description = "Accessories to complement mobile devices, including power banks and docking stations."
                }
            };
        }
    }
}