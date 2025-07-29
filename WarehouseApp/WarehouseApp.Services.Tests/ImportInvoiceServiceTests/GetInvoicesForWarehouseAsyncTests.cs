using MockQueryable;
using Moq;
using WarehouseApp.Web.ViewModels.ImportInvoice;
using WarehouseApp.Data.Models;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages;

namespace WarehouseApp.Services.Tests.ImportInvoiceServiceTests
{
    [TestFixture]
    public class GetInvoicesForWarehouseAsyncTests : ImportInvoiceServiceBaseTests
    {
        private AllImportInvoicesSearchFilterViewModel inputModel;
        private Warehouse warehouse;

        [SetUp]
        public void TestSetup()
        {
            inputModel = new AllImportInvoicesSearchFilterViewModel
            {
                WarehouseId = warehouseId,
                CurrentPage = 1,
                EntitiesPerPage = 5,
                SearchQuery = null,
                SupplierName = null,
                YearFilter = null,
            };

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
        }

        [Test]
        public async Task ReturnsSuccessAndSetsWarehouseName_WhenWarehouseFound()
        {
            userManager.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(validUser);

            appUserWarehouseRepo.Setup(x => x.GetWarehouseOwnedByUserAsync(warehouseId, userId))
                .ReturnsAsync(warehouse);

            var fakeInvoices = new List<ImportInvoice>
        {
            new ImportInvoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = "INV-001",
                Supplier = new Client { Name = "SupplierA", Address = "AddressA" },
                Date = new DateTime(2023, 1, 1),
                ImportInvoicesDetails = new List<ImportInvoiceDetail> { new ImportInvoiceDetail(), new ImportInvoiceDetail() }
            },
            new ImportInvoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = "INV-002",
                Supplier = new Client { Name = "SupplierB", Address = "AddressA" },
                Date = new DateTime(2023, 2, 1),
                ImportInvoicesDetails = new List<ImportInvoiceDetail> { new ImportInvoiceDetail() }
            }
        }.AsQueryable().BuildMock();

            importInvoiceRepo.Setup(x => x.GetAllForWarehouse(warehouseId))
                .Returns(fakeInvoices);

            var result = await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.WarehouseName, Is.EqualTo(warehouse.Name));
            Assert.That(inputModel.TotalInvoices, Is.EqualTo(fakeInvoices.Count()));

            Assert.That(inputModel.Invoices.Count(), Is.EqualTo(fakeInvoices.Count()));
            Assert.That(inputModel.Invoices.ElementAt(0).InvoiceNumber, Is.EqualTo("INV-002"));
            Assert.That(inputModel.Invoices.ElementAt(1).ProductCount, Is.EqualTo("2"));
        }

        [Test]
        public async Task ReturnsFailure_WhenUserNotFound()
        {
            SetupUserNotFound();

            var result = await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task ReturnsFailure_WhenWarehouseNotFoundOrNoPermission()
        {
            SetupWarehouseNotFound();

            var result = await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrWarehouseNotFound));
        }
    }
}