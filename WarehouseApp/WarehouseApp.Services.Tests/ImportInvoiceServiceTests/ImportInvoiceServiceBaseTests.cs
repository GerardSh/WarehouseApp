using Moq;
using Microsoft.AspNetCore.Identity;

using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data;
using WarehouseApp.Web.ViewModels.ImportInvoice;

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

        protected ApplicationUser validUser;

        [SetUp]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userManager = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

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
        }

        protected void SetupUserNotFound() =>
           userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync((ApplicationUser?)null);

        protected void SetupWarehouseNotFound() =>
        appUserWarehouseRepo.Setup(x => x.GetWarehouseOwnedByUserAsync(warehouseId, userId))
                .ReturnsAsync((Warehouse?)null);
    }
}
