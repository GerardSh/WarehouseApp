using Moq;
using Microsoft.Extensions.Logging;

using WarehouseApp.Data.Models;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoice;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

namespace WarehouseApp.Services.Tests.ExportInvoiceServiceTests
{
    [TestFixture]
    public class GetExportInvoiceForEditingAsyncTests : ExportInvoiceServiceBaseTests
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
            userManager.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await exportInvoiceService.GetExportInvoiceForEditingAsync(warehouseId, exportInvoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenWarehouseNotFound()
        {
            // Arrange
            appUserWarehouseRepo.Setup(r => r.GetWarehouseOwnedByUserAsync(warehouseId, userId))
                .ReturnsAsync((Warehouse?)null);

            // Act
            var result = await exportInvoiceService.GetExportInvoiceForEditingAsync(warehouseId, exportInvoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrWarehouseNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExportInvoiceNotFound()
        {
            // Arrange
            exportInvoiceRepo.Setup(r => r.GetExportInvoiceWithDetailsAsync(exportInvoiceId, warehouseId))
                .ReturnsAsync((ExportInvoice?)null);

            // Act
            var result = await exportInvoiceService.GetExportInvoiceForEditingAsync(warehouseId, exportInvoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrExportInvoiceNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            exportInvoiceRepo.Setup(r => r.GetExportInvoiceWithDetailsAsync(exportInvoiceId, warehouseId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await exportInvoiceService.GetExportInvoiceForEditingAsync(warehouseId, exportInvoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.ExportInvoice.GetModelFailure));

            logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(ErrorMessages.ExportInvoice.GetModelFailure)),
                It.Is<Exception>(ex => ex.Message == "Test exception"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Test]
        public async Task ShouldReturnCorrectModel_WhenValidData()
        {
            // Arrange
            exportInvoiceRepo.Setup(r => r.GetExportInvoiceWithDetailsAsync(exportInvoiceId, warehouseId))
                .ReturnsAsync(exportInvoice);

            // Act
            var result = await exportInvoiceService.GetExportInvoiceForEditingAsync(warehouseId, exportInvoiceId, userId);

            Assert.That(result.Success, Is.True);
            var model = result.Data!;

            // Assert
            Assert.That(model.Id, Is.EqualTo(exportInvoice.Id));
            Assert.That(model.InvoiceNumber, Is.EqualTo(exportInvoice.InvoiceNumber));
            Assert.That(model.Date, Is.EqualTo(exportInvoice.Date));
            Assert.That(model.WarehouseId, Is.EqualTo(exportInvoice.WarehouseId));

            Assert.That(model.ClientName, Is.EqualTo(exportInvoice.Client.Name));
            Assert.That(model.ClientAddress, Is.EqualTo(exportInvoice.Client.Address));
            Assert.That(model.ClientEmail, Is.EqualTo(exportInvoice.Client.Email));
            Assert.That(model.ClientPhoneNumber, Is.EqualTo(exportInvoice.Client.PhoneNumber));

            Assert.That(model.ExportedProducts.Count, Is.EqualTo(exportInvoice.ExportInvoicesDetails.Count));

            foreach (var exportedDetail in exportInvoice.ExportInvoicesDetails)
            {
                var importDetail = exportedDetail.ImportInvoiceDetail;
                var product = importDetail.Product;
                var category = product.Category;
                var importInvoice = importDetail.ImportInvoice;

                var vmDetail = model.ExportedProducts.FirstOrDefault(d => d.Id == exportedDetail.Id);

                Assert.That(vmDetail, Is.Not.Null);
                Assert.That(vmDetail!.ImportInvoiceNumber, Is.EqualTo(importInvoice.InvoiceNumber));
                Assert.That(vmDetail.ProductName, Is.EqualTo(product.Name));
                Assert.That(vmDetail.CategoryName, Is.EqualTo(category.Name));
                Assert.That(vmDetail.Quantity, Is.EqualTo(exportedDetail.Quantity));
                Assert.That(vmDetail.UnitPrice, Is.EqualTo(exportedDetail.UnitPrice));
            }
        }
    }
}
