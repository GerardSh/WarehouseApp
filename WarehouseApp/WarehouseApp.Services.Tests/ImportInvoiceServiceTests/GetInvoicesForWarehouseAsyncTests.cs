using MockQueryable;
using Moq;

using WarehouseApp.Web.ViewModels.ImportInvoice;
using WarehouseApp.Data.Models;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoice;
using static WarehouseApp.Common.Constants.ApplicationConstants;
using Microsoft.Extensions.Logging;

namespace WarehouseApp.Services.Tests.ImportInvoiceServiceTests
{
    [TestFixture]
    public class GetInvoicesForWarehouseAsyncTests : ImportInvoiceServiceBaseTests
    {
        private AllImportInvoicesSearchFilterViewModel inputModel;

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
        }

        [Test]
        public async Task ReturnsSuccessAndSetsWarehouseName_WhenWarehouseFound()
        {
            // Act
            var result = await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.WarehouseName, Is.EqualTo(warehouse.Name));
            Assert.That(inputModel.TotalInvoices, Is.EqualTo(invoices.Count()));

            Assert.That(inputModel.Invoices.Count(), Is.EqualTo(invoices.Count()));
            Assert.That(inputModel.Invoices.ElementAt(0).InvoiceNumber, Is.EqualTo("INV-002"));
            Assert.That(inputModel.Invoices.ElementAt(1).ProductCount, Is.EqualTo("2"));
        }

        [Test]
        public async Task ReturnsFailure_WhenUserNotFound()
        {
            // Arrange
            SetupUserNotFound();

            // Act
            var result = await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

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
            var result = await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.Warehouse.NoPermissionOrWarehouseNotFound));
        }

        [Test]
        public async Task AppliesSearchQueryFilterCorrectly()
        {
            // Arrange
            inputModel.SearchQuery = "INV-002";

            // Act
            var result = await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Invoices.Count(), Is.EqualTo(1));
            Assert.That(inputModel.Invoices.ElementAt(0).InvoiceNumber, Does.Contain("INV-002"));
        }

        [Test]
        public async Task AppliesSupplierNameFilterCorrectly()
        {
            // Arrange
            inputModel.SupplierName = "SupplierB";

            // Act
            var result = await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Invoices.Count(), Is.EqualTo(1));
            Assert.That(inputModel.Invoices.ElementAt(0).SupplierName, Does.Contain("SupplierB"));
        }

        [Test]
        public async Task AppliesYearFilterCorrectly()
        {
            // Arrange
            importInvoiceRepo.Setup(x => x.All())
                .Returns(invoices.AsQueryable().BuildMock());

            invoices.Add(new ImportInvoice()
            {
                Id = Guid.Parse("3FF7B60E-9C39-4E28-B2BD-35E750C6FBAE"),
                WarehouseId = warehouseId,
                InvoiceNumber = "INV-2020",
                Supplier = new Client { Name = "SupplierC", Address = "AddressC" },
                Date = new DateTime(2020, 1, 1),
                ImportInvoicesDetails = new List<ImportInvoiceDetail> {
                        new ImportInvoiceDetail(), new ImportInvoiceDetail() }
            });

            inputModel.YearFilter = "2020";

            // Act
            var result = await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Invoices.Count(), Is.EqualTo(1));
            Assert.That(inputModel.Invoices.ElementAt(0).InvoiceNumber, Is.EqualTo("INV-2020"));
            Assert.That(inputModel.Invoices.ElementAt(0).Id, Is.EqualTo(invoices.ElementAt(2).Id.ToString()));

            inputModel.YearFilter = "2021-2025";

            // Act
            result = await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Invoices.Count(), Is.EqualTo(2));
            Assert.That(inputModel.Invoices.ElementAt(0).Date, Is.EqualTo(invoices[1].Date.ToString(DateFormat)));
            Assert.That(inputModel.Invoices.ElementAt(0).Id, Is.EqualTo(invoices[1].Id.ToString()));
            Assert.That(inputModel.Invoices.ElementAt(1).Date, Is.EqualTo(invoices[0].Date.ToString(DateFormat)));
            Assert.That(inputModel.Invoices.ElementAt(1).Id, Is.EqualTo(invoices[0].Id.ToString()));
        }

        [Test]
        public async Task GetInvoicesForWarehouseAsync_NegativeEntitiesPerPage_SetsToDefault()
        {
            // Arrange
            inputModel.EntitiesPerPage = -10;

            // Act
            var result = await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(inputModel.EntitiesPerPage, Is.EqualTo(5));
        }

        [Test]
        public async Task EntitiesPerPage_AboveMaximum_CapsTo100()
        {
            // Arrange
            inputModel.EntitiesPerPage = 1000;

            // Act
            var result = await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(inputModel.EntitiesPerPage, Is.EqualTo(100));
        }

        [Test]
        public async Task CurrentPage_GreaterThanTotalPages_AdjustsToTotalPages()
        {
            // Arrange
            inputModel.CurrentPage = 99;

            // Act
            var result = await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(inputModel.CurrentPage, Is.EqualTo(inputModel.TotalPages));
        }

        [Test]
        public async Task YearFilter_InvalidFormat_IgnoresFilter()
        {
            // Arrange
            inputModel.YearFilter = "abc";

            // Act
            var result = await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.TotalItemsBeforePagination, Is.EqualTo(2));
            Assert.That(inputModel.Invoices.Count(), Is.EqualTo(invoices.Count()));
        }

        [Test]
        public async Task NoInvoicesFound_SetsTotalPagesToZero_AndCurrentPageToOne()
        {
            // Arrange
            invoices.Clear();
            importInvoiceRepo.Setup(x => x.All())
                .Returns(invoices.AsQueryable().BuildMock());

            // Act
            var result = await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.TotalPages, Is.EqualTo(0));
            Assert.That(inputModel.CurrentPage, Is.EqualTo(1));
            Assert.That(inputModel.TotalItemsBeforePagination, Is.EqualTo(0));
            Assert.That(inputModel.Invoices, Is.Empty);
        }

        [TestCase(0, -1, 5, 1)]
        [TestCase(150, 1, 100, 1)]
        [TestCase(10, 1000, 10, 1)]
        [TestCase(10, 0, 10, 1)]
        [TestCase(5, 1, 5, 1)]
        public async Task PaginationValidation_ForImportInvoices(
            int entitiesPerPageInput,
            int currentPageInput,
            int expectedEntitiesPerPage,
            int expectedCurrentPage)
        {
            // Arrange
            inputModel.EntitiesPerPage = entitiesPerPageInput;
            inputModel.CurrentPage = currentPageInput;

            // Act
            var result = await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.EntitiesPerPage, Is.EqualTo(expectedEntitiesPerPage));
            Assert.That(inputModel.CurrentPage, Is.EqualTo(expectedCurrentPage));
        }

        [Test]
        public async Task ReturnsFailureOnException()
        {
            // Arrange
            userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                           .ThrowsAsync(new Exception("Simulated failure"));

            // Act
            var result = await importInvoiceService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(RetrievingFailure));

            logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(RetrievingFailure)),
                It.Is<Exception>(ex => ex.Message == "Simulated failure"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}