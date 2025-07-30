using Moq;
using Microsoft.AspNetCore.Identity;

using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data;
using MockQueryable;

namespace WarehouseApp.Services.Tests.ImportInvoiceServiceTests
{
    [TestFixture]
    public abstract class ImportInvoiceServiceBaseTests
    {
        protected Mock<UserManager<ApplicationUser>> userManager;
        protected Mock<IImportInvoiceRepository> importInvoiceRepo;
        protected Mock<IImportInvoiceDetailRepository> importInvoiceDetailRepo;
        protected Mock<IExportInvoiceDetailRepository> exportInvoiceDetailRepo;
        protected Mock<IApplicationUserWarehouseRepository> appUserWarehouseRepo;

        protected Mock<IClientService> clientService;
        protected Mock<ICategoryService> categoryService;
        protected Mock<IProductService> productService;

        protected ImportInvoiceService importInvoiceService;

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

            importInvoiceRepo = new Mock<IImportInvoiceRepository>();
            importInvoiceDetailRepo = new Mock<IImportInvoiceDetailRepository>();
            exportInvoiceDetailRepo = new Mock<IExportInvoiceDetailRepository>();
            appUserWarehouseRepo = new Mock<IApplicationUserWarehouseRepository>();

            clientService = new Mock<IClientService>();
            categoryService = new Mock<ICategoryService>();
            productService = new Mock<IProductService>();

            importInvoiceService = new ImportInvoiceService(
                userManager.Object,
                importInvoiceRepo.Object,
                importInvoiceDetailRepo.Object,
                exportInvoiceDetailRepo.Object,
                appUserWarehouseRepo.Object,
                clientService.Object,
                categoryService.Object,
                productService.Object);

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
                    InvoiceNumber = "INV-001",
                    Supplier = new Client { Name = "SupplierA", Address = "AddressA" },
                    Date = new DateTime(2023, 1, 1),
                    ImportInvoicesDetails = new List<ImportInvoiceDetail> {
                        new ImportInvoiceDetail(), new ImportInvoiceDetail() }
                },
                new ImportInvoice
                {
                    Id = Guid.Parse("2FF7B60E-9C39-4E28-B2BD-35E750C6FBAE"),
                    InvoiceNumber = "INV-002",
                    Supplier = new Client { Name = "SupplierB", Address = "AddressB" },
                    Date = new DateTime(2025, 2, 1),
                    ImportInvoicesDetails = new List<ImportInvoiceDetail> {
                        new ImportInvoiceDetail() }
                }
            };

            importInvoiceRepo.Setup(r => r.GetAllForWarehouse(warehouseId))
               .Returns(invoices.AsQueryable().BuildMock());
        }

        protected void SetupUserNotFound() =>
           userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync((ApplicationUser?)null);

        protected void SetupWarehouseNotFound() =>
        appUserWarehouseRepo.Setup(x => x.GetWarehouseOwnedByUserAsync(warehouseId, userId))
                .ReturnsAsync((Warehouse?)null);
    }
}
