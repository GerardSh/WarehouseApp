using MockQueryable;

using WarehouseApp.Data.Models;

namespace WarehouseApp.Services.Tests.StockServiceTests
{
    [TestFixture]
    public class GetAvailableQuantityAsyncTests : StockServiceBaseTests
    {
        [Test]
        public async Task ReturnsCorrectAvailable_WhenExportDetailsExist()
        {
            // Arrange
            var importDetail = invoice1.ImportInvoicesDetails.First();
            var exportQuantity = importDetail.ExportInvoicesPerProduct.Sum(e => e.Quantity);
            var expectedAvailable = importDetail.Quantity - exportQuantity;

            importInvoiceDetailRepo.Setup(x => x.All())
                .Returns(new List<ImportInvoiceDetail> { importDetail }.AsQueryable().BuildMock());

            exportInvoiceDetailRepo.Setup(x => x.All())
                .Returns(importDetail.ExportInvoicesPerProduct.AsQueryable().BuildMock());

            // Act
            var result = await stockService.GetAvailableQuantityAsync(importDetail.Id);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(expectedAvailable));
        }

        [Test]
        public async Task ExcludesExportDetail_WhenExcludedIdProvided()
        {
            // Arrange
            var importDetail = invoice1.ImportInvoicesDetails.First();
            var allExports = importDetail.ExportInvoicesPerProduct;

            var exportToExclude = allExports.First();
            var remainingExports = allExports.Where(e => e.Id != exportToExclude.Id);
            var expectedAvailable = importDetail.Quantity - remainingExports.Sum(e => e.Quantity);

            importInvoiceDetailRepo.Setup(x => x.All())
                .Returns(new List<ImportInvoiceDetail> { importDetail }.AsQueryable().BuildMock());

            exportInvoiceDetailRepo.Setup(x => x.All())
                .Returns(allExports.AsQueryable().BuildMock());

            // Act
            var result = await stockService.GetAvailableQuantityAsync(importDetail.Id, exportToExclude.Id);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(expectedAvailable));
        }

        [Test]
        public async Task ReturnsFullImport_WhenNoExportsExist()
        {
            // Arrange
            var importDetail = invoice2.ImportInvoicesDetails.First();

            importInvoiceDetailRepo.Setup(x => x.All())
                .Returns(new List<ImportInvoiceDetail> { importDetail }.AsQueryable().BuildMock());

            exportInvoiceDetailRepo.Setup(x => x.All())
                .Returns(new List<ExportInvoiceDetail>().AsQueryable().BuildMock());

            // Act
            var result = await stockService.GetAvailableQuantityAsync(importDetail.Id);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.EqualTo(importDetail.Quantity));
        }
    }
}
