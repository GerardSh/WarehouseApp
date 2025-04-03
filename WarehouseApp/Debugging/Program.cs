using Microsoft.EntityFrameworkCore;
using WarehouseApp.Data;

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

            // Delete if it exists and create the database
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            Console.WriteLine("Database created");
        }
    }
}
