using Moq;
using MockQueryable.Moq;
using System.Linq.Expressions;

using WarehouseApp.Data.Models;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoice;
using Microsoft.Extensions.Logging;
using MockQueryable;

namespace WarehouseApp.Services.Tests.ImportInvoiceServiceTests
{
    [TestFixture]
    public class DeleteImportInvoiceAsyncTests : ImportInvoiceServiceBaseTests
    {
        private Guid invoiceId = Guid.NewGuid();
        private string invoiceNumber = "Inv001";

        [Test]
        public async Task ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            userManager.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await importInvoiceService.DeleteImportInvoiceAsync(warehouseId, invoiceId, userId);

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
            var result = await importInvoiceService.DeleteImportInvoiceAsync(warehouseId, invoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.Warehouse.NoPermissionOrWarehouseNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenInvoiceNotFound()
        {
            // Arrange
            importInvoiceRepo.Setup(r => r.AllTracked())
                .Returns(new List<ImportInvoice>().AsQueryable().BuildMock());

            // Act
            var result = await importInvoiceService.DeleteImportInvoiceAsync(warehouseId, invoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrImportInvoiceNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExportInvoicesExist()
        {
            // Arrange
            var invoice = new ImportInvoice { Id = invoiceId, WarehouseId = warehouseId, InvoiceNumber = invoiceNumber };

            importInvoiceRepo.Setup(r => r.AllTracked())
                .Returns(new List<ImportInvoice> { invoice }.AsQueryable().BuildMock());

            exportInvoiceDetailRepo.Setup(r => r.ExistsAsync(
                It.IsAny<Expression<Func<ExportInvoiceDetail, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await importInvoiceService.DeleteImportInvoiceAsync(warehouseId, invoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ExistingExportInvoices));
        }

        [Test]
        public async Task ShouldDeleteInvoice_WhenNoExportInvoicesExist()
        {
            // Arrange
            var invoice = new ImportInvoice { Id = invoiceId, WarehouseId = warehouseId, InvoiceNumber = invoiceNumber };

            importInvoiceRepo.Setup(r => r.AllTracked())
                .Returns(new List<ImportInvoice> { invoice }.AsQueryable().BuildMock());

            exportInvoiceDetailRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<ExportInvoiceDetail, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var result = await importInvoiceService.DeleteImportInvoiceAsync(warehouseId, invoiceId, userId);

            // Assert
            importInvoiceRepo.Verify(r => r.Delete(invoice), Times.Once);
            importInvoiceRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            var invoice = new ImportInvoice { Id = invoiceId, WarehouseId = warehouseId, InvoiceNumber = invoiceNumber };

            importInvoiceRepo.Setup(r => r.AllTracked())
                .Returns(new List<ImportInvoice> { invoice }.AsQueryable().BuildMock());

            exportInvoiceDetailRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<ExportInvoiceDetail, bool>>>()))
                .ThrowsAsync(new Exception("Unexpected"));

            // Act
            var result = await importInvoiceService.DeleteImportInvoiceAsync(warehouseId, invoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(DeletionFailure));

            logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains(DeletionFailure)),
                It.Is<Exception>(ex => ex.Message == "Unexpected"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}