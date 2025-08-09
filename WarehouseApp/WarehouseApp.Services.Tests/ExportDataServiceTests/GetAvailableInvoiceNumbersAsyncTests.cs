using Moq;
using Microsoft.Extensions.Logging;

using WarehouseApp.Services.Data.Dtos.ImportInvoices;
using WarehouseApp.Services.Data.Models;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportData;

namespace WarehouseApp.Services.Tests.ExportDataServiceTests
{
    [TestFixture]
    public class GetAvailableInvoiceNumbersAsyncTests : ExportDataServiceBaseTests
    {
        List<ImportInvoiceSummaryDto> invoicesDto;

        [SetUp]
        public void TestSetup()
        {
            invoicesDto = new List<ImportInvoiceSummaryDto>
            {
                new ImportInvoiceSummaryDto
                {
                    Id = Guid.NewGuid(),
                    InvoiceNumber = "INV-001",
                    ImportDetails = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
                },
                new ImportInvoiceSummaryDto
                {
                    Id = Guid.NewGuid(),
                    InvoiceNumber = "INV-002",
                    ImportDetails = new List<Guid> { Guid.NewGuid() }
                }
            };
        }

        [Test]
        public async Task ReturnsFailure_WhenUserNotFound()
        {
            // Arrange
            SetupUserNotFound();

            // Act
            var result = await exportDataService.GetAvailableInvoiceNumbersAsync(warehouseId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task ReturnsFailure_WhenWarehouseNotFoundOrNoPermission()
        {
            // Arrange
            SetupWarehouseNotFound();

            var result = await exportDataService.GetAvailableInvoiceNumbersAsync(warehouseId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrWarehouseNotFound));
        }

        [Test]
        public async Task ReturnsFailure_WhenImportInvoiceServiceFails()
        {
            // Arrange
            importInvoiceService.Setup(s => s.GetInvoicesByWarehouseIdAsync(warehouseId))
                .ReturnsAsync(OperationResult<IEnumerable<ImportInvoiceSummaryDto>>.Failure("Something failed"));

            // Act
            var result = await exportDataService.GetAvailableInvoiceNumbersAsync(warehouseId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Something failed"));
        }

        [Test]
        public async Task ReturnsEmptyList_WhenNoInvoicesHaveAvailableStock()
        {
            // Arrange
            importInvoiceService.Setup(s => s.GetInvoicesByWarehouseIdAsync(warehouseId))
                .ReturnsAsync(OperationResult<IEnumerable<ImportInvoiceSummaryDto>>.Ok(invoicesDto));


            stockService.Setup(s => s.GetAvailableQuantityAsync(It.IsAny<Guid>(), It.IsAny<Guid?>()))
                .ReturnsAsync(OperationResult<int>.Ok(0));

            // Act
            var result = await exportDataService.GetAvailableInvoiceNumbersAsync(warehouseId, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Empty);
        }

        [Test]
        public async Task ReturnsInvoiceNumbers_WithOneInvoiceHasAvailableStock()
        {
            // Arrange
            importInvoiceService.Setup(s => s.GetInvoicesByWarehouseIdAsync(warehouseId))
                .ReturnsAsync(OperationResult<IEnumerable<ImportInvoiceSummaryDto>>.Ok(invoicesDto));

            stockService.SetupSequence(s => s.GetAvailableQuantityAsync(It.IsAny<Guid>(), It.IsAny<Guid?>()))
                .ReturnsAsync(OperationResult<int>.Ok(0))
                .ReturnsAsync(OperationResult<int>.Ok(5))
                .ReturnsAsync(OperationResult<int>.Ok(0));

            // Act
            var result = await exportDataService.GetAvailableInvoiceNumbersAsync(warehouseId, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EquivalentTo(new[] { invoicesDto[0].InvoiceNumber }));
        }

        [Test]
        public async Task ReturnsInvoiceNumbers_WithTwoIvnoicesWithAvailableStock()
        {
            // Arrange
            importInvoiceService.Setup(s => s.GetInvoicesByWarehouseIdAsync(warehouseId))
                .ReturnsAsync(OperationResult<IEnumerable<ImportInvoiceSummaryDto>>.Ok(invoicesDto));

            stockService.SetupSequence(s => s.GetAvailableQuantityAsync(It.IsAny<Guid>(), It.IsAny<Guid?>()))
                .ReturnsAsync(OperationResult<int>.Ok(0))
                .ReturnsAsync(OperationResult<int>.Ok(1))
                .ReturnsAsync(OperationResult<int>.Ok(1));

            // Act
            var result = await exportDataService.GetAvailableInvoiceNumbersAsync(warehouseId, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EquivalentTo(new[] { invoicesDto[0].InvoiceNumber, invoicesDto[1].InvoiceNumber }));
        }

        [Test]
        public async Task ReturnsFailure_WhenExceptionIsThrown()
        {
            // Arrange
            importInvoiceService.Setup(s => s.GetInvoicesByWarehouseIdAsync(warehouseId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await exportDataService.GetAvailableInvoiceNumbersAsync(warehouseId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(RetrievingInvoicesFailure));

            logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(RetrievingInvoicesFailure)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
