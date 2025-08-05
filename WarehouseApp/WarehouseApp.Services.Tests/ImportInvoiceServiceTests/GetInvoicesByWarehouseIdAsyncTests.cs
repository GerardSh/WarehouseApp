using MockQueryable;

using WarehouseApp.Data.Models;
using WarehouseApp.Common.OutputMessages;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoice;

namespace WarehouseApp.Services.Tests.ImportInvoiceServiceTests
{
    [TestFixture]
    public class GetInvoicesByWarehouseIdAsyncTests : ImportInvoiceServiceBaseTests
    {
        [Test]
        public async Task ShouldReturnInvoicesOrderedByDateDescending()
        {
            // Act
            var result = await importInvoiceService.GetInvoicesByWarehouseIdAsync(warehouseId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            var invoicesList = result.Data.ToList();

            var sortedInvoices = invoices.OrderByDescending(i => i.Date).ToList();

            Assert.That(invoicesList.Count, Is.EqualTo(sortedInvoices.Count));

            for (int i = 0; i < invoicesList.Count; i++)
            {
                Assert.That(invoicesList[i].Id, Is.EqualTo(sortedInvoices[i].Id));
                Assert.That(invoicesList[i].InvoiceNumber, Is.EqualTo(sortedInvoices[i].InvoiceNumber));

                var expectedDetailsIds = sortedInvoices[i].ImportInvoicesDetails.Select(d => d.Id);
                Assert.That(invoicesList[i].ImportDetails, Is.EqualTo(expectedDetailsIds));
            }
        }

        [Test]
        public async Task ShouldReturnFailure_WhenNoInvoicesFound()
        {
            // Arrange
            var emptyQueryable = new List<ImportInvoice>().AsQueryable().BuildMock();
            importInvoiceRepo.Setup(r => r.All()).Returns(emptyQueryable);

            // Act
            var result = await importInvoiceService.GetInvoicesByWarehouseIdAsync(warehouseId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoInvoicesFound));
            Assert.That(result.Data, Is.Null);
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            importInvoiceRepo.Setup(r => r.All()).Throws(new Exception("Database failure"));

            // Act
            var result = await importInvoiceService.GetInvoicesByWarehouseIdAsync(warehouseId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.ImportInvoice.RetrievingFailure));
            Assert.That(result.Data, Is.Null);
        }
    }
}
