using Moq;
using MockQueryable;

using WarehouseApp.Data.Models;
using WarehouseApp.Web.ViewModels.Stock;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;
using Microsoft.Extensions.Logging;

namespace WarehouseApp.Services.Tests.StockServiceTests
{
    [TestFixture]
    public class GetInvoicesForWarehouseAsyncTests : StockServiceBaseTests
    {
        AllProductsSearchFilterViewModel inputModel;

        [SetUp]
        public void TestSetup()
        {
            inputModel = new AllProductsSearchFilterViewModel()
            {
                WarehouseId = warehouseId,
                WarehouseName = warehouse.Name,
            };

            var importDetails = invoice1.ImportInvoicesDetails
                .Concat(invoice2.ImportInvoicesDetails)
                .ToList();

            importInvoiceDetailRepo.Setup(x => x.All())
                .Returns(importDetails.AsQueryable().BuildMock());
        }

        [Test]
        public async Task ReturnsFailure_WhenUserNotFound()
        {
            // Arrange
            SetupUserNotFound();

            // Act
            var result = await stockService.GetInvoicesForWarehouseAsync(inputModel, userId);

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
            var result = await stockService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrWarehouseNotFound));
        }

        [Test]
        public async Task ReturnsAllProducts_WhenIncludeExportedProductsIsTrue()
        {
            // Arrange
            var exportDetail = invoice1.ImportInvoicesDetails.ElementAt(1).ExportInvoicesPerProduct.ElementAt(0);
            exportDetail.Quantity = 200;

            inputModel.IncludeExportedProducts = true;

            // Act
            var result = await stockService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Products.Count(), Is.EqualTo(2));
            Assert.That(inputModel.Products.ElementAt(0).Available, Is.EqualTo(150));
            Assert.That(inputModel.Products.ElementAt(0).TotalImported, Is.EqualTo(250));
            Assert.That(inputModel.Products.ElementAt(0).TotalExported, Is.EqualTo(100));
            Assert.That(inputModel.Products.ElementAt(1).Available, Is.EqualTo(0));
            Assert.That(inputModel.Products.ElementAt(1).TotalImported, Is.EqualTo(200));
            Assert.That(inputModel.Products.ElementAt(1).TotalExported, Is.EqualTo(200));
            Assert.That(inputModel.WarehouseName, Is.EqualTo(warehouse.Name));
            Assert.That(inputModel.WarehouseId, Is.EqualTo(warehouseId));
            Assert.That(inputModel.TotalItemsBeforePagination, Is.EqualTo(2));
            Assert.That(inputModel.TotalProducts, Is.EqualTo(1));
        }

        [Test]
        public async Task ReturnsOnlyAvailableProducts_WhenIncludeExportedProductsIsFalse()
        {
            // Arrange
            var exportDetail = invoice1.ImportInvoicesDetails.ElementAt(1).ExportInvoicesPerProduct.ElementAt(0);
            exportDetail.Quantity = 200;

            inputModel.IncludeExportedProducts = false;

            // Act
            var result = await stockService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Products.Count(), Is.EqualTo(1));
            Assert.That(inputModel.Products.ElementAt(0).Available, Is.EqualTo(150));
            Assert.That(inputModel.Products.ElementAt(0).TotalImported, Is.EqualTo(250));
            Assert.That(inputModel.Products.ElementAt(0).TotalExported, Is.EqualTo(100));
            Assert.That(inputModel.TotalItemsBeforePagination, Is.EqualTo(1));
            Assert.That(inputModel.TotalProducts, Is.EqualTo(1));
        }

        [Test]
        public async Task FiltersProducts_ByProductQuery()
        {
            // Arrange
            inputModel.ProductQuery = "Product A";

            // Act
            var result = await stockService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Products.Count(), Is.EqualTo(1));
            Assert.That(inputModel.Products.First().ProductName, Does.Contain("Product A"));
            Assert.That(inputModel.Products.First().CategoryName, Does.Contain("Category A"));
        }

        [Test]
        public async Task FiltersProducts_ByCategoryQuery()
        {
            // Arrange
            inputModel.CategoryQuery = "Category A";

            // Act
            var result = await stockService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Products.Count(), Is.EqualTo(1));
            Assert.That(inputModel.Products.First().ProductName, Does.Contain("Product A"));
            Assert.That(inputModel.Products.First().CategoryName, Does.Contain("Category A"));
        }

        [Test]
        public async Task FiltersProducts_ByProductAndCategoryQueries()
        {
            // Arrange
            inputModel.CategoryQuery = "Category B";

            // Act
            var result = await stockService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Products.Count(), Is.EqualTo(1));
            Assert.That(inputModel.Products.First().ProductName, Does.Contain("Product B"));
            Assert.That(inputModel.Products.First().CategoryName, Does.Contain("Category B"));
        }

        [Test]
        public async Task FiltersProducts_WhenProductAndCategoryQueriesAreNull()
        {
            // Arrange
            inputModel.ProductQuery = null;
            inputModel.CategoryQuery = null;

            // Act
            var result = await stockService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Products.Count(), Is.EqualTo(2));
            Assert.That(inputModel.TotalItemsBeforePagination, Is.EqualTo(2));
            Assert.That(inputModel.TotalProducts, Is.EqualTo(2));
        }

        [Test]
        public async Task ReturnsEmptyProducts_WhenFiltersExcludeAllProducts()
        {
            // Arrange
            inputModel.ProductQuery = "NonExistentProduct";
            inputModel.CategoryQuery = "NonExistentCategory";

            // Act
            var result = await stockService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Products, Is.Empty);
            Assert.That(inputModel.TotalItemsBeforePagination, Is.EqualTo(0));
            Assert.That(inputModel.TotalPages, Is.EqualTo(0));
            Assert.That(inputModel.CurrentPage, Is.EqualTo(1));
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
            var result = await stockService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.EntitiesPerPage, Is.EqualTo(expectedEntitiesPerPage));
            Assert.That(inputModel.CurrentPage, Is.EqualTo(expectedCurrentPage));
        }

        [Test]
        public async Task AppliesPagination_Correctly()
        {
            // Arrange
            inputModel.EntitiesPerPage = 1;
            inputModel.CurrentPage = 2;

            // Act
            var result = await stockService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Products.Count(), Is.EqualTo(1));
            Assert.That(inputModel.CurrentPage, Is.EqualTo(2));
            Assert.That(inputModel.TotalPages, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public async Task ReturnsEmptyProducts_WhenNoImportDetailsExist()
        {
            // Arrange
            importInvoiceDetailRepo.Setup(x => x.All())
                .Returns(new List<ImportInvoiceDetail>().AsQueryable().BuildMock());
            inputModel.CurrentPage = 2;

            // Act
            var result = await stockService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Products, Is.Empty);
            Assert.That(inputModel.TotalProducts, Is.EqualTo(0));
            Assert.That(inputModel.TotalItemsBeforePagination, Is.EqualTo(0));
            Assert.That(inputModel.TotalPages, Is.EqualTo(0));
            Assert.That(inputModel.CurrentPage, Is.EqualTo(1));
            Assert.That(inputModel.WarehouseName, Is.EqualTo(warehouse.Name));
        }

        [Test]
        public async Task ReturnsFailureOnException()
        {
            // Arrange
            userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                           .ThrowsAsync(new Exception("Simulated failure"));

            // Act
            var result = await stockService.GetInvoicesForWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.Stock.RetrievingFailure));

            logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(ErrorMessages.Stock.RetrievingFailure)),
                It.Is<Exception>(ex => ex.Message == "Simulated failure"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
