using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WarehouseApp.Data.Models;

namespace WarehouseApp.Data
{
    public class WarehouseDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public WarehouseDbContext()
        {
        }

        public WarehouseDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<ImportInvoice> ImportInvoices { get; set; }

        public virtual DbSet<ImportInvoiceDetail> ImportInvoiceDetails { get; set; }

        public virtual DbSet<ExportInvoice> ExportInvoices { get; set; }

        public virtual DbSet<ExportInvoiceDetail> ExportInvoiceDetails { get; set; }

        public virtual DbSet<Client> Clients { get; set; }

        public virtual DbSet<Warehouse> Warehouses { get; set; }

        public virtual DbSet<ApplicationUserWarehouse> UsersWarehouses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
