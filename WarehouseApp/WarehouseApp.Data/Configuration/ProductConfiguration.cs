using WarehouseApp.Data.Models;
using static WarehouseApp.Common.Constants.EntityConstants.Product;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WarehouseApp.Data.Configuration
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> entity)
        {
            entity
                .HasKey(p => p.Id);

            entity
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            entity
                .Property(p => p.Description)
                .IsRequired(false)
                .HasMaxLength(DescriptionMaxLength);

            entity
                .Property(p => p.CategoryId)
                .IsRequired();

            entity
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasData(SeedProducts());
        }

        private List<Product> SeedProducts()
        {
            return new List<Product>
            {
                // Category: Computers & Peripherals (31bf5b32-a3f7-4e37-8230-2202a081ea87)
                new Product
                {
                    Id = Guid.Parse("a6a5c1b8-d5bc-46b5-b75c-0152e35e344e"),
                    Name = "Wireless Mouse",
                    Description = "Ergonomic wireless mouse with adjustable DPI settings.",
                    CategoryId = Guid.Parse("31bf5b32-a3f7-4e37-8230-2202a081ea87")
                },
                new Product
                {
                    Id = Guid.Parse("72c4bc6d-72f5-4b43-b299-cb1bcaa4cd93"),
                    Name = "Mechanical Keyboard",
                    Description = "High-quality mechanical keyboard with customizable RGB lighting.",
                    CategoryId = Guid.Parse("31bf5b32-a3f7-4e37-8230-2202a081ea87")
                },
                new Product
                {
                    Id = Guid.Parse("d9b37f82-0b4f-4c92-8821-f4b3b5dcd045"),
                    Name = "Gaming Headset",
                    Description = "Over-ear gaming headset with noise cancellation and surround sound.",
                    CategoryId = Guid.Parse("31bf5b32-a3f7-4e37-8230-2202a081ea87")
                },
                new Product
                {
                    Id = Guid.Parse("c3a9d582-74b6-4512-91a5-2643a1b682be"),
                    Name = "USB-C Docking Station",
                    Description = "Multi-port USB-C hub with HDMI, Ethernet, and USB ports.",
                    CategoryId = Guid.Parse("31bf5b32-a3f7-4e37-8230-2202a081ea87")
                },
                new Product
                {
                    Id = Guid.Parse("c8f3a7d2-1b6e-4c5d-92f4-a0b5d4e7c9f1"),
                    Name = "4K Monitor",
                    Description = "27-inch 4K UHD monitor with HDR and IPS panel.",
                    CategoryId = Guid.Parse("31bf5b32-a3f7-4e37-8230-2202a081ea87")
                },
                new Product
                {
                    Id = Guid.Parse("d4c7e1a5-3b92-4f86-92d1-0b5f3a7c4682"),
                    Name = "Graphics Tablet",
                    Description = "Professional-grade graphics tablet with pressure-sensitive stylus.",
                    CategoryId = Guid.Parse("31bf5b32-a3f7-4e37-8230-2202a081ea87")
                },
                
                // Category: Home Appliances (e3edc37f-6ba9-4616-b0c5-98c33451bcdb)
                new Product
                {
                    Id = Guid.Parse("e7a2c5d4-3b9f-4871-82c1-d5b4a0f6e398"),
                    Name = "Bluetooth Speaker",
                    Description = "Compact Bluetooth speaker with deep bass and long battery life.",
                    CategoryId = Guid.Parse("e3edc37f-6ba9-4616-b0c5-98c33451bcdb")
                },
                new Product
                {
                    Id = Guid.Parse("f1a3b5c7-92d4-4f86-8b1d-0b5e4c3d7982"),
                    Name = "Smart Light Bulb",
                    Description = "Wi-Fi-enabled smart light bulb with color-changing options.",
                    CategoryId = Guid.Parse("e3edc37f-6ba9-4616-b0c5-98c33451bcdb")
                },
                new Product
                {
                    Id = Guid.Parse("a4b3c7d5-9e1f-4f86-bc2d-0b5d4e3c7981"),
                    Name = "Portable Coffee Maker",
                    Description = "Compact coffee maker for brewing on the go.",
                    CategoryId = Guid.Parse("e3edc37f-6ba9-4616-b0c5-98c33451bcdb")
                },
                
                // Category: Mobile Accessories (26ea2ad2-8d2c-4c43-939f-d7d818572aff)
                new Product
                {
                    Id = Guid.Parse("f1a5b3c7-74d9-4e8f-92b1-c3d2a0b5f8e6"),
                    Name = "Smartphone Holder",
                    Description = "Flexible gooseneck smartphone holder for hands-free use.",
                    CategoryId = Guid.Parse("26ea2ad2-8d2c-4c43-939f-d7d818572aff")
                },
                new Product
                {
                    Id = Guid.Parse("b4c9d2e7-1a3f-4856-bc91-f5d3a0b7e462"),
                    Name = "Wireless Charger",
                    Description = "Fast wireless charger compatible with Qi-enabled devices.",
                    CategoryId = Guid.Parse("26ea2ad2-8d2c-4c43-939f-d7d818572aff")
                },
                new Product
                {
                    Id = Guid.Parse("b7c4d5e2-3a92-4f86-91b1-7d3a0b5e4683"),
                    Name = "Portable Power Bank",
                    Description = "20000mAh portable power bank with fast charging capabilities.",
                    CategoryId = Guid.Parse("26ea2ad2-8d2c-4c43-939f-d7d818572aff")
                },
                
                // Category: Office Furniture & Accessories (3daadd5e-f5c9-4222-9878-1a02faa44339)
                new Product
                {
                    Id = Guid.Parse("b5e21436-3c45-4f52-a854-874bcf356f9d"),
                    Name = "Laptop Stand",
                    Description = "Adjustable aluminum laptop stand for improved ergonomics.",
                    CategoryId = Guid.Parse("3daadd5e-f5c9-4222-9878-1a02faa44339")
                },
                new Product
                {
                    Id = Guid.Parse("a0b5d4c3-7e92-4f81-bc2d-5f3a7d1e4689"),
                    Name = "Ergonomic Office Chair",
                    Description = "Adjustable office chair with lumbar support.",
                    CategoryId = Guid.Parse("3daadd5e-f5c9-4222-9878-1a02faa44339")
                },
                new Product
                {
                    Id = Guid.Parse("f3a5b7d2-1e9c-4f86-bc2d-0b5d4c3e7891"),
                    Name = "Mechanical Pencil Set",
                    Description = "Premium mechanical pencil set for precise writing and drawing.",
                    CategoryId = Guid.Parse("3daadd5e-f5c9-4222-9878-1a02faa44339")
                },
                
                // Category: Storage & Backup (dde109fb-71c5-4f59-8810-3bf4057c5351)
                new Product
                {
                    Id = Guid.Parse("ed2b3c7f-8a45-4d9e-80f4-b1c7d2a0f9b2"),
                    Name = "Portable SSD",
                    Description = "1TB portable SSD with high-speed data transfer.",
                    CategoryId = Guid.Parse("dde109fb-71c5-4f59-8810-3bf4057c5351")
                },
                new Product
                {
                    Id = Guid.Parse("b5c4d7e2-3a92-4f81-9b1f-7d3a0b5e4682"),
                    Name = "External Hard Drive",
                    Description = "2TB external hard drive for secure data backup.",
                    CategoryId = Guid.Parse("dde109fb-71c5-4f59-8810-3bf4057c5351")
                },
                
                // Category: Wearable Tech (314820d6-a4d4-4873-8dec-95e5a6904a8e)
                new Product
                {
                    Id = Guid.Parse("d1c7e4a5-3b92-4f86-bc7d-1a0b5f3e4826"),
                    Name = "Smartwatch",
                    Description = "Fitness-focused smartwatch with heart rate monitoring.",
                    CategoryId = Guid.Parse("314820d6-a4d4-4873-8dec-95e5a6904a8e")
                },
                new Product
                {
                    Id = Guid.Parse("a5b3d7c2-9e1f-4f86-bc2d-0b5d4c3e7981"),
                    Name = "VR Headset",
                    Description = "Next-gen VR headset with high-resolution display and tracking.",
                    CategoryId = Guid.Parse("314820d6-a4d4-4873-8dec-95e5a6904a8e")
                },
                new Product
                {
                    Id = Guid.Parse("d2c7e5a4-3b91-4f86-92d1-0b5a3f7c4682"),
                    Name = "Noise-Canceling Earbuds",
                    Description = "True wireless earbuds with active noise cancellation.",
                    CategoryId = Guid.Parse("314820d6-a4d4-4873-8dec-95e5a6904a8e")
                }
            };
        }
    }
}
