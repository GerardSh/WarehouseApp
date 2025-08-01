using MockQueryable;
using Moq;

using WarehouseApp.Web.ViewModels.ExportInvoice;
using WarehouseApp.Data.Models;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoice;
using static WarehouseApp.Common.Constants.ApplicationConstants;

namespace WarehouseApp.Services.Tests.ExportInvoiceServiceTests
{
    [TestFixture]
    public class GetInvoicesForWarehouseAsync : ExportInvoiceServiceBaseTests
    {
        private AllExportInvoicesSearchFilterViewModel inputModel;

        [SetUp]
        public void TestSetup()
        {
            inputModel = new AllExportInvoicesSearchFilterViewModel
            {
                WarehouseId = warehouseId,
                CurrentPage = 1,
                EntitiesPerPage = 5,
                SearchQuery = null,
                ClientName = null,
                YearFilter = null,
            };
        }

        [Test]
        public async Task ReturnsSuccessAndSetsWarehouseName_WhenWarehouseFound()
        {
            // Act
            var result = await exportInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.WarehouseName, Is.EqualTo(warehouse.Name));
            Assert.That(inputModel.TotalInvoices, Is.EqualTo(exportInvoices.Count()));
            Assert.That(inputModel.TotalItemsBeforePagination, Is.EqualTo(exportInvoices.Count()));

            Assert.That(inputModel.Invoices.Count(), Is.EqualTo(exportInvoices.Count()));
            Assert.That(inputModel.Invoices.ElementAt(0).InvoiceNumber, Is.EqualTo("EXP-002"));
            Assert.That(inputModel.Invoices.ElementAt(1).ExportedProductsCount, Is.EqualTo("2"));
            Assert.That(inputModel.Invoices.ElementAt(0).ExportedProductsCount, Is.EqualTo("1"));
        }

        [Test]
        public async Task ReturnsFailure_WhenUserNotFound()
        {
            // Arrange
            SetupUserNotFound();

            // Act
            var result = await exportInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task ReturnsFailure_WhenWarehouseNotFoundOrNoPermission()
        {
            // Arrange
            SetupWarehouseNotFound();

            // Act
            var result = await exportInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.Warehouse.NoPermissionOrWarehouseNotFound));
        }

        [Test]
        public async Task AppliesSearchQueryFilterCorrectly()
        {
            // Arrange
            inputModel.SearchQuery = "EXP-002";

            // Act
            var result = await exportInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Invoices.Count(), Is.EqualTo(1));
            Assert.That(inputModel.Invoices.ElementAt(0).InvoiceNumber, Does.Contain("EXP-002"));
        }

        [Test]
        public async Task AppliesSupplierNameFilterCorrectly()
        {
            // Arrange
            inputModel.ClientName = "ClientB";

            // Act
            var result = await exportInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Invoices.Count(), Is.EqualTo(1));
            Assert.That(inputModel.Invoices.ElementAt(0).ClientName, Does.Contain("ClientB"));
        }

        [Test]
        public async Task AppliesYearFilterCorrectly()
        {
            // Arrange
            exportInvoices.Add(new ExportInvoice()
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = "EXP-2020",
                Date = new DateTime(2020, 1, 1),
                Client = new Client() { Id = Guid.NewGuid(), Address = "Address3", Name = "ClientC" },
                Warehouse = warehouse,
                WarehouseId = warehouse.Id
            });

            inputModel.YearFilter = "2020";

            // Act
            var result = await exportInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Invoices.Count(), Is.EqualTo(1));
            Assert.That(inputModel.Invoices.ElementAt(0).InvoiceNumber, Is.EqualTo("EXP-2020"));
            Assert.That(inputModel.Invoices.ElementAt(0).Id, Is.EqualTo(exportInvoices.ElementAt(2).Id.ToString()));

            inputModel.YearFilter = "2021-2025";

            // Act
            result = await exportInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Invoices.Count(), Is.EqualTo(2));
            Assert.That(inputModel.Invoices.ElementAt(0).Date, Is.EqualTo(exportInvoices[1].Date.ToString(DateFormat)));
            Assert.That(inputModel.Invoices.ElementAt(0).Id, Is.EqualTo(exportInvoices[1].Id.ToString()));
            Assert.That(inputModel.Invoices.ElementAt(1).Date, Is.EqualTo(exportInvoices[0].Date.ToString(DateFormat)));
            Assert.That(inputModel.Invoices.ElementAt(1).Id, Is.EqualTo(exportInvoices[0].Id.ToString()));
        }

        [TestCase(0, -1, 5, 1)]
        [TestCase(150, 1, 100, 1)]
        [TestCase(99, 1, 99, 1)]
        [TestCase(10, 1000, 10, 1)]
        [TestCase(10, 0, 10, 1)]
        [TestCase(5, 1, 5, 1)]
        public async Task PaginationValidation_ForExportInvoices(
            int entitiesPerPageInput,
            int currentPageInput,
            int expectedEntitiesPerPage,
            int expectedCurrentPage)
        {
            // Arrange
            inputModel.EntitiesPerPage = entitiesPerPageInput;
            inputModel.CurrentPage = currentPageInput;

            // Act
            var result = await exportInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.EntitiesPerPage, Is.EqualTo(expectedEntitiesPerPage));
            Assert.That(inputModel.CurrentPage, Is.EqualTo(expectedCurrentPage));
        }

        [Test]
        public async Task YearFilter_InvalidFormat_IgnoresFilter()
        {
            // Arrange
            inputModel.YearFilter = "abc";

            // Act
            var result = await exportInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.TotalItemsBeforePagination, Is.EqualTo(2));
            Assert.That(inputModel.Invoices.Count(), Is.EqualTo(invoices.Count()));
        }

        [Test]
        public async Task NoInvoicesFound_SetsTotalPagesToZero_AndCurrentPageToOne()
        {
            // Arrange
            exportInvoices.Clear();
            exportInvoiceRepo.Setup(x => x.All())
                .Returns(exportInvoices.AsQueryable().BuildMock());

            // Act
            var result = await exportInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.TotalPages, Is.EqualTo(0));
            Assert.That(inputModel.CurrentPage, Is.EqualTo(1));
            Assert.That(inputModel.TotalItemsBeforePagination, Is.EqualTo(0));
            Assert.That(inputModel.Invoices, Is.Empty);
        }

        [Test]
        public async Task ReturnsFailureOnException()
        {
            // Arrange
            userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                           .ThrowsAsync(new Exception("Simulated failure"));

            // Act
            var result = await exportInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(RetrievingFailure));
        }
    }
}
