using Moq;
using Microsoft.AspNetCore.Identity;

using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data;

namespace WarehouseApp.Services.Tests.ExportDataServiceTests
{
    [TestFixture]
    public abstract class ExportDataServiceBaseTests
    {
        protected Mock<UserManager<ApplicationUser>> userManager;
        protected Mock<IApplicationUserWarehouseRepository> appUserWarehouseRepo;

        protected Mock<IImportInvoiceService> importInvoiceService;
        protected Mock<IStockService> stockService;

        protected ExportDataService exportDataService;

        protected static readonly Guid userId = Guid.Parse("C994999B-02DD-46C2-ABC4-00C4787E629F");

        protected static readonly Guid warehouseId = Guid.Parse("A1F7B60E-9C39-4E28-B2BD-35E750C6FBAE");

        protected List<ImportInvoice> invoices = null!;

        protected Warehouse warehouse = null!;

        protected ApplicationUser validUser;

        [SetUp]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            validUser = new ApplicationUser { Id = userId };

            userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
               .ReturnsAsync(validUser);

            appUserWarehouseRepo = new Mock<IApplicationUserWarehouseRepository>();

            importInvoiceService = new Mock<IImportInvoiceService>();
            stockService = new Mock<IStockService>();


            exportDataService = new ExportDataService(
                userManager.Object,
                importInvoiceService.Object,
                stockService.Object,
                appUserWarehouseRepo.Object);

            warehouse = new Warehouse
            {
                Id = Guid.Parse("A1F7B60E-9C39-4E28-B2BD-35E750C6FBAE"),
                Name = "Alpha Warehouse",
                Address = "123 Alpha St",
                CreatedDate = new DateTime(2022, 5, 1),
                Size = 100,
                WarehouseUsers = new List<ApplicationUserWarehouse> {
                        new ApplicationUserWarehouse { ApplicationUserId = userId }
                    }
            };

            appUserWarehouseRepo.Setup(x => x.GetWarehouseOwnedByUserAsync(warehouseId, userId))
               .ReturnsAsync(warehouse);

            invoices = new List<ImportInvoice>
            {
                new ImportInvoice
                {
                    Id = Guid.Parse("1FF7B60E-9C39-4E28-B2BD-35E750C6FBAE"),
                    WarehouseId = warehouseId,
                    InvoiceNumber = "INV-001",
                    Supplier = new Client { Name = "SupplierA", Address = "AddressA" },
                    Date = new DateTime(2023, 1, 1),
                    ImportInvoicesDetails = new List<ImportInvoiceDetail> {
                        new ImportInvoiceDetail(), new ImportInvoiceDetail() }
                },
                new ImportInvoice
                {
                    Id = Guid.Parse("2FF7B60E-9C39-4E28-B2BD-35E750C6FBAE"),
                    WarehouseId = warehouseId,
                    InvoiceNumber = "INV-002",
                    Supplier = new Client { Name = "SupplierB", Address = "AddressB" },
                    Date = new DateTime(2025, 2, 1),
                    ImportInvoicesDetails = new List<ImportInvoiceDetail> {
                        new ImportInvoiceDetail() }
                }
            };
        }

        protected void SetupUserNotFound() =>
           userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync((ApplicationUser?)null);

        protected void SetupWarehouseNotFound() =>
        appUserWarehouseRepo.Setup(x => x.GetWarehouseOwnedByUserAsync(warehouseId, userId))
                .ReturnsAsync((Warehouse?)null);
    }
}
