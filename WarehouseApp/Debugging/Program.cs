using Microsoft.EntityFrameworkCore;
using WarehouseApp.Data;

namespace Debugging
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Get the connection string from the configuration
            var connectionString = "Server=.;Database=WarehouseAppDebbuging;User Id=sa;Password=r3F4iJbYas&#aRj^bmjj;TrustServerCertificate=true;";

            // Set up DbContext with the connection string
            var optionsBuilder = new DbContextOptionsBuilder<WarehouseDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // Instantiate the DbContext with the connection string
            var dbContext = new WarehouseDbContext(optionsBuilder.Options);

            // Delete if it exists and create the database
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            Console.WriteLine("Database created");


//private List<Product> SeedProducts()
//        {
//            return new List<Product>
//            {
//                new Product
//                {
//                    Id = Guid.Parse("a6a5c1b8-d5bc-46b5-b75c-0152e35e344e"),
//                    ProductName = "Wireless Mouse",
//                    Description = "Ergonomic wireless mouse with adjustable DPI settings."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("72c4bc6d-72f5-4b43-b299-cb1bcaa4cd93"),
//                    ProductName = "Mechanical Keyboard",
//                    Description = "High-quality mechanical keyboard with customizable RGB lighting."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("d9b37f82-0b4f-4c92-8821-f4b3b5dcd045"),
//                    ProductName = "Gaming Headset",
//                    Description = "Over-ear gaming headset with noise cancellation and surround sound."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("b5e21436-3c45-4f52-a854-874bcf356f9d"),
//                    ProductName = "Laptop Stand",
//                    Description = "Adjustable aluminum laptop stand for improved ergonomics."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("c3a9d582-74b6-4512-91a5-2643a1b682be"),
//                    ProductName = "USB-C Docking Station",
//                    Description = "Multi-port USB-C hub with HDMI, Ethernet, and USB ports."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("ed2b3c7f-8a45-4d9e-80f4-b1c7d2a0f9b2"),
//                    ProductName = "Portable SSD",
//                    Description = "1TB portable SSD with high-speed data transfer."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("f1a5b3c7-74d9-4e8f-92b1-c3d2a0b5f8e6"),
//                    ProductName = "Smartphone Holder",
//                    Description = "Flexible gooseneck smartphone holder for hands-free use."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("b4c9d2e7-1a3f-4856-bc91-f5d3a0b7e462"),
//                    ProductName = "Wireless Charger",
//                    Description = "Fast wireless charger compatible with Qi-enabled devices."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("e7a2c5d4-3b9f-4871-82c1-d5b4a0f6e398"),
//                    ProductName = "Bluetooth Speaker",
//                    Description = "Compact Bluetooth speaker with deep bass and long battery life."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("c8f3a7d2-1b6e-4c5d-92f4-a0b5d4e7c9f1"),
//                    ProductName = "4K Monitor",
//                    Description = "27-inch 4K UHD monitor with HDR and IPS panel."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("d1c7e4a5-3b92-4f86-bc7d-1a0b5f3e4826"),
//                    ProductName = "Smartwatch",
//                    Description = "Fitness-focused smartwatch with heart rate monitoring."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("a0b5d4c3-7e92-4f81-bc2d-5f3a7d1e4689"),
//                    ProductName = "Ergonomic Office Chair",
//                    Description = "Adjustable office chair with lumbar support."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("b5c4d7e2-3a92-4f81-9b1f-7d3a0b5e4682"),
//                    ProductName = "External Hard Drive",
//                    Description = "2TB external hard drive for secure data backup."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("f3a5b7d2-1e9c-4f86-bc2d-0b5d4c3e7891"),
//                    ProductName = "Mechanical Pencil Set",
//                    Description = "Premium mechanical pencil set for precise writing and drawing."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("d4c7e1a5-3b92-4f86-92d1-0b5f3a7c4682"),
//                    ProductName = "Graphics Tablet",
//                    Description = "Professional-grade graphics tablet with pressure-sensitive stylus."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("a5b3d7c2-9e1f-4f86-bc2d-0b5d4c3e7981"),
//                    ProductName = "VR Headset",
//                    Description = "Next-gen VR headset with high-resolution display and tracking."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("b7c4d5e2-3a92-4f86-91b1-7d3a0b5e4683"),
//                    ProductName = "Portable Power Bank",
//                    Description = "20000mAh portable power bank with fast charging capabilities."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("f1a3b5c7-92d4-4f86-8b1d-0b5e4c3d7982"),
//                    ProductName = "Smart Light Bulb",
//                    Description = "Wi-Fi-enabled smart light bulb with color-changing options."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("d2c7e5a4-3b91-4f86-92d1-0b5a3f7c4682"),
//                    ProductName = "Noise-Canceling Earbuds",
//                    Description = "True wireless earbuds with active noise cancellation."
//                },
//                new Product
//                {
//                    Id = Guid.Parse("a4b3c7d5-9e1f-4f86-bc2d-0b5d4e3c7981"),
//                    ProductName = "Portable Coffee Maker",
//                    Description = "Compact coffee maker for brewing on the go."
//                }
//            };
        }
    }
}
