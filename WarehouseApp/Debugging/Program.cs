using Microsoft.EntityFrameworkCore;
using WarehouseApp.Data;
using WarehouseApp.Data.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Debugging
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Get the connection string from the configuration
            var connectionString = "Server=.;Database=WarehouseApp;User Id=sa;Password=r3F4iJbYas&#aRj^bmjj;TrustServerCertificate=true;";

            // Set up DbContext with the connection string
            var optionsBuilder = new DbContextOptionsBuilder<WarehouseDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // Instantiate the DbContext with the connection string
            var dbContext = new WarehouseDbContext(optionsBuilder.Options);

            var imports = dbContext.ImportInvoices
                .Include(e => e.ImportInvoicesDetails)
                .ThenInclude(i => i.Product)
                .Include(e => e.ImportInvoicesDetails)
                .ThenInclude(e => e.ExportInvoicesPerProduct)
                .ThenInclude(e => e.ExportInvoice)
                .Select(e => new
                {
                    e.InvoiceNumber,
                    AvailableProducts = e.ImportInvoicesDetails.Select(ie => new
                    {
                        ProductName = ie.Product.Name,
                        ie.Quantity,
                        TakenQuantity = ie.ExportInvoicesPerProduct.Sum(e => e.Quantity),
                        AvailableQuantity = ie.Quantity - ie.ExportInvoicesPerProduct.Sum(e => e.Quantity)
                    })
                })
                .ToList();
        }
    }
}
