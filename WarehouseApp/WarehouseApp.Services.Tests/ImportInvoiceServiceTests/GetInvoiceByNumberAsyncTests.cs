using Moq;
using MockQueryable;

using WarehouseApp.Data.Models;
using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoice;

namespace WarehouseApp.Services.Tests.ImportInvoiceServiceTests
{
    [TestFixture]
    public class GetInvoiceByNumberAsyncTests : ImportInvoiceServiceBaseTests
    {
        private readonly string invoiceNumber = "INV-12345";

        [Test]
        public async Task ShouldReturnInvoice_WhenFound()
        {
            // Arrange
            var invoice = new ImportInvoice
            {
                Id = Guid.NewGuid(),
                WarehouseId = warehouseId,
                InvoiceNumber = invoiceNumber,
                ImportInvoicesDetails = new List<ImportInvoiceDetail>
                {
                    new ImportInvoiceDetail
                    {
                        Id = Guid.NewGuid(),
                        Product = new Product
                        {
                            Name = "Product",
                            Category = new Category { Id = Guid.NewGuid(), Name = "Cat1" }
                        }
                    }
                }
            };

            var mockInvoices = new List<ImportInvoice> { invoice }.AsQueryable().BuildMock();

            importInvoiceRepo.Setup(r => r.All())
                .Returns(mockInvoices);

            // Act
            var result = await importInvoiceService.GetInvoiceByNumberAsync(warehouseId, invoiceNumber);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Id, Is.EqualTo(invoice.Id));
            Assert.That(result.Data.InvoiceNumber, Is.EqualTo(invoiceNumber));
            Assert.That(result.Data.ImportInvoicesDetails.ElementAt(0).Product.Name, Is.EqualTo("Product"));
            Assert.That(result.Data.ImportInvoicesDetails.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenInvoiceNotFound()
        {
            // Arrange
            var emptyQueryable = new List<ImportInvoice>().AsQueryable().BuildMock();
            importInvoiceRepo.Setup(r => r.All()).Returns(emptyQueryable);

            // Act
            var result = await importInvoiceService.GetInvoiceByNumberAsync(warehouseId, invoiceNumber);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrImportInvoiceNotFound));
            Assert.That(result.Data, Is.Null);
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            importInvoiceRepo.Setup(r => r.All()).Throws(new Exception("Database failure"));

            // Act
            var result = await importInvoiceService.GetInvoiceByNumberAsync(warehouseId, invoiceNumber);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(GetModelFailure));
            Assert.That(result.Data, Is.Null);
        }
    }
}
