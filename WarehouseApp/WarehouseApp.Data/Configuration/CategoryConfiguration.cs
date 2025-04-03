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
        }
    }
}