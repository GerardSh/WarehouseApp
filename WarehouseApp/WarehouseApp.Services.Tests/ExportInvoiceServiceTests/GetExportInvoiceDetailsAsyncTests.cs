using Moq;

using WarehouseApp.Data.Models;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoice;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

namespace WarehouseApp.Services.Tests.ExportInvoiceServiceTests
{
    [TestFixture]
    public class GetExportInvoiceDetailsAsyncTests : ExportInvoiceServiceBaseTests
    {
        private Guid exportInvoiceId;
        private ExportInvoice exportInvoice;

        [SetUp]
        public void SetupTest()
        {
            exportInvoice = exportInvoices.ElementAt(0);

            exportInvoiceId = exportInvoice.Id;

            exportInvoiceRepo.Setup(r => r.GetExportInvoiceWithDetailsAsync(exportInvoiceId, warehouseId))
                .ReturnsAsync(exportInvoice);
        }

        [Test]
        public async Task ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            SetupUserNotFound();

            // Act
            var result = await exportInvoiceService.GetExportInvoiceDetailsAsync(warehouseId, exportInvoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenWarehouseNotFound()
        {
            // Arrange
            SetupWarehouseNotFound();

            // Act
            var result = await exportInvoiceService.GetExportInvoiceDetailsAsync(warehouseId, exportInvoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrWarehouseNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExportInvoiceNotFound()
        {
            // Arrange
            var nonExistendId = Guid.NewGuid();

            exportInvoiceRepo.Setup(r => r.GetExportInvoiceWithDetailsAsync(nonExistendId, warehouseId))
                .ReturnsAsync((ExportInvoice?)null);

            // Act
            var result = await exportInvoiceService.GetExportInvoiceDetailsAsync(warehouseId, nonExistendId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrExportInvoiceNotFound));
        }

        [Test]
        public async Task ShouldReturnSuccessWithCorrectViewModel_WhenValidDataProvided()
        {
            // Act
            var result = await exportInvoiceService.GetExportInvoiceDetailsAsync(warehouseId, exportInvoiceId, userId);

            // Assert: Top-level result
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            var vm = result.Data;

            Assert.That(vm.Id, Is.EqualTo(exportInvoice.Id));
            Assert.That(vm.InvoiceNumber, Is.EqualTo(exportInvoice.InvoiceNumber));
            Assert.That(vm.Date, Is.EqualTo(exportInvoice.Date));

            Assert.That(vm.ClientName, Is.EqualTo(exportInvoice.Client.Name));
            Assert.That(vm.ClientAddress, Is.EqualTo(exportInvoice.Client.Address));
            Assert.That(vm.ClientPhone, Is.EqualTo(exportInvoice.Client.PhoneNumber));
            Assert.That(vm.ClientEmail, Is.EqualTo(exportInvoice.Client.Email));

            Assert.That(vm.ExportedProducts.Count, Is.EqualTo(exportInvoice.ExportInvoicesDetails.Count));

            foreach (var exportedDetail in exportInvoice.ExportInvoicesDetails)
            {
                var vmDetail = vm.ExportedProducts.FirstOrDefault(d =>
                    d.ProductName == exportedDetail.ImportInvoiceDetail.Product.Name &&
                    d.Quantity == exportedDetail.Quantity);

                Assert.That(vmDetail, Is.Not.Null);

                var importDetail = exportedDetail.ImportInvoiceDetail;
                var product = importDetail.Product;
                var category = product.Category;
                var importInvoice = importDetail.ImportInvoice;

                Assert.That(vmDetail!.ImportInvoiceNumber, Is.EqualTo(importInvoice.InvoiceNumber));
                Assert.That(vmDetail.ProductName, Is.EqualTo(product.Name));
                Assert.That(vmDetail.ProductDescription, Is.EqualTo(product.Description));
                Assert.That(vmDetail.CategoryName, Is.EqualTo(category.Name));
                Assert.That(vmDetail.CategoryDescription, Is.EqualTo(category.Description));
                Assert.That(vmDetail.Quantity, Is.EqualTo(exportedDetail.Quantity));
                Assert.That(vmDetail.UnitPrice, Is.EqualTo(exportedDetail.UnitPrice));
                Assert.That(vmDetail.Total, Is.EqualTo(exportedDetail.Quantity * exportedDetail.UnitPrice));
            }
        }

        [Test]
        public async Task ReturnsNA_WhenOptionalClientProductOrCategoryFieldsAreNull()
        {
            // Arrange
            exportInvoice.Client.PhoneNumber = null;
            exportInvoice.Client.Email = null;
            exportInvoice.ExportInvoicesDetails.ElementAt(0).ImportInvoiceDetail.Product.Description = null;
            exportInvoice.ExportInvoicesDetails.ElementAt(0).ImportInvoiceDetail.Product.Category.Description = null;

            // Act
            var result = await exportInvoiceService.GetExportInvoiceDetailsAsync(warehouseId, exportInvoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            Assert.That(result.Data!.ClientPhone, Is.EqualTo("N/A"));
            Assert.That(result.Data!.ClientEmail, Is.EqualTo("N/A"));

            Assert.That(result.Data!.ExportedProducts[0].ProductDescription, Is.EqualTo("N/A"));
            Assert.That(result.Data!.ExportedProducts[0].CategoryDescription, Is.EqualTo("N/A"));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            exportInvoiceRepo.Setup(r => r.GetExportInvoiceWithDetailsAsync(exportInvoiceId, warehouseId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await exportInvoiceService.GetExportInvoiceDetailsAsync(warehouseId, exportInvoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.ExportInvoice.GetModelFailure));
        }
    }
}
