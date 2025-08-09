using Moq;
using MockQueryable;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data;

namespace WarehouseApp.Services.Tests.ExportInvoiceServiceTests
{
    [TestFixture]
    public abstract class ExportInvoiceServiceBaseTests
    {
        protected Mock<UserManager<ApplicationUser>> userManager;
        protected Mock<IImportInvoiceRepository> importInvoiceRepo;
        protected Mock<IImportInvoiceDetailRepository> importInvoiceDetailRepo;
        protected Mock<IExportInvoiceRepository> exportInvoiceRepo;
        protected Mock<IExportInvoiceDetailRepository> exportInvoiceDetailRepo;
        protected Mock<IApplicationUserWarehouseRepository> appUserWarehouseRepo;
        protected Mock<ILogger<ExportInvoiceService>> logger;

        protected Mock<IClientService> clientService;
        protected Mock<IStockService> stockService;

        protected ExportInvoiceService exportInvoiceService;

        protected static readonly Guid userId = Guid.Parse("C994999B-02DD-46C2-ABC4-00C4787E629F");

        protected static readonly Guid warehouseId = Guid.Parse("A1F7B60E-9C39-4E28-B2BD-35E750C6FBAE");

        protected List<ImportInvoice> invoices = null!;

        protected List<ExportInvoice> exportInvoices = null!;

        protected Warehouse warehouse = null!;

        protected ApplicationUser validUser = null!;

        protected Client exportClient = null!;

        protected ImportInvoiceDetail importInvoiceDetail1 = null!;
        protected ImportInvoiceDetail importInvoiceDetail2 = null!;
        protected ExportInvoiceDetail exportInvoiceDetail1 = null!;
        protected ExportInvoiceDetail exportInvoiceDetail2 = null!;

        [SetUp]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            validUser = new ApplicationUser { Id = userId };

            userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
               .ReturnsAsync(validUser);

            importInvoiceRepo = new Mock<IImportInvoiceRepository>();
            importInvoiceDetailRepo = new Mock<IImportInvoiceDetailRepository>();
            exportInvoiceRepo = new Mock<IExportInvoiceRepository>();
            exportInvoiceDetailRepo = new Mock<IExportInvoiceDetailRepository>();
            appUserWarehouseRepo = new Mock<IApplicationUserWarehouseRepository>();

            clientService = new Mock<IClientService>();
            stockService = new Mock<IStockService>();

            logger = new Mock<ILogger<ExportInvoiceService>>();

            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));

            exportInvoiceService = new ExportInvoiceService(
                userManager.Object,
                importInvoiceRepo.Object,
                importInvoiceDetailRepo.Object,
                exportInvoiceRepo.Object,
                exportInvoiceDetailRepo.Object,
                appUserWarehouseRepo.Object,
                clientService.Object,
                stockService.Object,
                logger.Object);

            warehouse = new Warehouse
            {
                Id = Guid.Parse("A1F7B60E-9C39-4E28-B2BD-35E750C6FBAE"),
                Name = "Alpha Warehouse",
                Address = "123 Alpha St",
                CreatedDate = new DateTime(2022, 5, 1),
                Size = 100,
                WarehouseUsers = new List<ApplicationUserWarehouse>
                {
                   new ApplicationUserWarehouse { ApplicationUserId = userId }
                }
            };

            appUserWarehouseRepo.Setup(x => x.GetWarehouseOwnedByUserAsync(warehouseId, userId))
               .ReturnsAsync(warehouse);

            var category1 = new Category { Id = Guid.NewGuid(), Name = "Category A", Description = "Description A" };
            var category2 = new Category { Id = Guid.NewGuid(), Name = "Category B", Description = "Description B" };

            var product1 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product A",
                Category = category1,
                CategoryId = category1.Id,
                Description = "Description A"
            };

            var product2 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product B",
                Category = category2,
                CategoryId = category2.Id,
                Description = "Description B"
            };

            var client1 = new Client
            {
                Id = Guid.NewGuid(),
                Name = "SupplierA",
                Address = "AddressA"
            };

            var client2 = new Client
            {
                Id = Guid.NewGuid(),
                Name = "SupplierB",
                Address = "AddressB"
            };

            var detail1 = new ImportInvoiceDetail
            {
                Id = Guid.NewGuid(),
                Product = product1,
                ProductId = product1.Id,
                Quantity = 100,
                UnitPrice = 10.5m,
            };

            var detail2 = new ImportInvoiceDetail
            {
                Id = Guid.NewGuid(),
                Product = product2,
                ProductId = product2.Id,
                Quantity = 200,
                UnitPrice = 20.0m
            };

            var detail3 = new ImportInvoiceDetail
            {
                Id = Guid.NewGuid(),
                Product = product1,
                ProductId = product1.Id,
                Quantity = 150,
                UnitPrice = 9.5m
            };

            var importInvoice1 = new ImportInvoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = "INV-001",
                Date = new DateTime(2023, 1, 1),
                Supplier = client1,
                SupplierId = client1.Id,
                Warehouse = warehouse,
                WarehouseId = warehouse.Id,
                ImportInvoicesDetails = new List<ImportInvoiceDetail> { detail1, detail2 }
            };

            var importInvoice2 = new ImportInvoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = "INV-002",
                Date = new DateTime(2025, 2, 1),
                Supplier = client2,
                SupplierId = client2.Id,
                Warehouse = warehouse,
                WarehouseId = warehouse.Id,
                ImportInvoicesDetails = new List<ImportInvoiceDetail> { detail3 }
            };

            importInvoiceDetail1 = detail1;
            importInvoiceDetail2 = detail2;

            detail1.ImportInvoice = importInvoice1;
            detail1.ImportInvoiceId = importInvoice1.Id;

            detail2.ImportInvoice = importInvoice1;
            detail2.ImportInvoiceId = importInvoice1.Id;

            detail3.ImportInvoice = importInvoice2;
            detail3.ImportInvoiceId = importInvoice2.Id;

            invoices = new List<ImportInvoice> { importInvoice1, importInvoice2 };

            importInvoiceRepo.Setup(r => r.All())
               .Returns(invoices.AsQueryable().BuildMock());

            exportClient = new Client
            {
                Id = Guid.NewGuid(),
                Name = "ClientA",
                Address = "Export Address",
                PhoneNumber = "0888888888",
                Email = "test@email.com"
            };

            var exportClient2 = new Client
            {
                Id = Guid.NewGuid(),
                Name = "ClientB",
                Address = "Export Address2"
            };

            var exportInvoice1 = new ExportInvoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = "EXP-001",
                Date = new DateTime(2025, 6, 1),
                Client = exportClient,
                ClientId = exportClient.Id,
                Warehouse = warehouse,
                WarehouseId = warehouse.Id
            };

            var exportInvoice2 = new ExportInvoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = "EXP-002",
                Date = new DateTime(2025, 7, 1),
                Client = exportClient2,
                ClientId = exportClient2.Id,
                Warehouse = warehouse,
                WarehouseId = warehouse.Id
            };

            var exportDetail1 = new ExportInvoiceDetail
            {
                Id = Guid.NewGuid(),
                ExportInvoice = exportInvoice1,
                ExportInvoiceId = exportInvoice1.Id,
                ImportInvoiceDetail = detail1,
                ImportInvoiceDetailId = detail1.Id,
                Quantity = 30,
                UnitPrice = 12.0m
            };

            var exportDetail2 = new ExportInvoiceDetail
            {
                Id = Guid.NewGuid(),
                ExportInvoice = exportInvoice1,
                ExportInvoiceId = exportInvoice1.Id,
                ImportInvoiceDetail = detail2,
                ImportInvoiceDetailId = detail2.Id,
                Quantity = 50,
                UnitPrice = 25.0m
            };

            var exportDetail3 = new ExportInvoiceDetail
            {
                Id = Guid.NewGuid(),
                ExportInvoice = exportInvoice2,
                ExportInvoiceId = exportInvoice2.Id,
                ImportInvoiceDetail = detail3,
                ImportInvoiceDetailId = detail3.Id,
                Quantity = 70,
                UnitPrice = 11.0m
            };

            exportInvoiceDetail1 = exportDetail1;
            exportInvoiceDetail2 = exportDetail2;

            exportInvoice1.ExportInvoicesDetails.Add(exportDetail1);
            exportInvoice1.ExportInvoicesDetails.Add(exportDetail2);
            exportInvoice2.ExportInvoicesDetails.Add(exportDetail3);

            detail1.ExportInvoicesPerProduct.Add(exportDetail1);
            detail2.ExportInvoicesPerProduct.Add(exportDetail2);
            detail3.ExportInvoicesPerProduct.Add(exportDetail3);

            exportInvoices = new List<ExportInvoice> { exportInvoice1, exportInvoice2 };

            exportInvoiceRepo.Setup(r => r.All())
               .Returns(exportInvoices.AsQueryable().BuildMock());

            userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(validUser);

            appUserWarehouseRepo.Setup(x => x.GetWarehouseOwnedByUserAsync(warehouseId, userId))
                .ReturnsAsync(warehouse);
        }

        protected void SetupUserNotFound() =>
           userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync((ApplicationUser?)null);

        protected void SetupWarehouseNotFound() =>
        appUserWarehouseRepo.Setup(x => x.GetWarehouseOwnedByUserAsync(warehouseId, userId))
                .ReturnsAsync((Warehouse?)null);
    }
}
