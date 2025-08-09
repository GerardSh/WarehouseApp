using Moq;
using MockQueryable;
using Microsoft.Extensions.Logging;

using WarehouseApp.Data.Models;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoice;

namespace WarehouseApp.Services.Tests.ExportInvoiceServiceTests
{
    [TestFixture]
    public class DeleteExportInvoiceAsyncTests : ExportInvoiceServiceBaseTests
    {
        private Guid exportInvoiceId;
        private ExportInvoice exportInvoice;

        [SetUp]
        public void TestSetup()
        {
            exportInvoice = exportInvoices[0];
            exportInvoiceId = exportInvoice.Id;
        }

        [Test]
        public async Task ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            userManager.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await exportInvoiceService.DeleteExportInvoiceAsync(warehouseId, exportInvoiceId, userId);

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
            var result = await exportInvoiceService.DeleteExportInvoiceAsync(warehouseId, exportInvoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.Warehouse.NoPermissionOrWarehouseNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExportInvoiceNotFound()
        {
            // Arrange
            exportInvoiceRepo.Setup(r => r.AllTracked())
                .Returns(new List<ExportInvoice>().AsQueryable().BuildMock());

            // Act
            var result = await exportInvoiceService.DeleteExportInvoiceAsync(warehouseId, exportInvoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrExportInvoiceNotFound));
        }

        [Test]
        public async Task ShouldDeleteInvoice_WhenNoExportInvoicesExist()
        {
            // Arrange
            exportInvoiceRepo.Setup(r => r.AllTracked())
               .Returns(exportInvoices.AsQueryable().BuildMock());

            // Act
            var result = await exportInvoiceService.DeleteExportInvoiceAsync(warehouseId, exportInvoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            exportInvoiceRepo.Setup(r => r.AllTracked())
               .Returns(exportInvoices.AsQueryable().BuildMock());

            exportInvoiceRepo.Setup(r => r.SaveChangesAsync())
                .ThrowsAsync(new Exception("Unexpected"));

            // Act
            var result = await exportInvoiceService.DeleteExportInvoiceAsync(warehouseId, exportInvoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(DeletionFailure));

            logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(DeletionFailure)),
                It.Is<Exception>(ex => ex.Message == "Unexpected"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }
    }
}