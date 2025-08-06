using Moq;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Dtos.ImportInvoices;
using WarehouseApp.Services.Data.Models;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

namespace WarehouseApp.Services.Tests.ExportDataServiceTests
{
    [TestFixture]
    public class GetAvailableProductsForInvoiceAsyncTests : ExportDataServiceBaseTests
    {
        List<ProductDto> productsDto;
        ImportInvoice invoice;

        [SetUp]
        public void TestSetup()
        {
            var productCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Electronics",
                Description = "Electronic items"
            };

            var productA = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Laptop",
                Description = "Gaming laptop",
                CategoryId = productCategory.Id,
                Category = productCategory
            };

            var productB = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Monitor",
                Description = "4K Monitor",
                CategoryId = productCategory.Id,
                Category = productCategory
            };

            var invoiceDetailA = new ImportInvoiceDetail
            {
                Id = Guid.NewGuid(),
                Product = productA,
                Quantity = 5
            };

            var invoiceDetailB = new ImportInvoiceDetail
            {
                Id = Guid.NewGuid(),
                Product = productB,
                Quantity = 5
            };

            invoice = new ImportInvoice
            {
                Id = Guid.Parse("1FF7B60E-9C39-4E28-B2BD-35E750C6FBAE"),
                WarehouseId = warehouseId,
                InvoiceNumber = "INV-001",
                Supplier = new Client
                {
                    Name = "Supplier A",
                    Address = "123 Supplier St."
                },
                Date = new DateTime(2023, 1, 1),
                ImportInvoicesDetails = new List<ImportInvoiceDetail>
                {
                    invoiceDetailA,
                    invoiceDetailB
                }
            };
        }

        [Test]
        public async Task ReturnsFailure_WhenUserNotFound()
        {
            // Arrange
            SetupUserNotFound();

            // Act
            var result = await exportDataService.GetAvailableProductsForInvoiceAsync(warehouseId, userId, invoice.InvoiceNumber);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task ReturnsFailure_WhenWarehouseNotFoundOrNoPermission()
        {
            // Arrange
            SetupWarehouseNotFound();

            var result = await exportDataService.GetAvailableProductsForInvoiceAsync(warehouseId, userId, invoice.InvoiceNumber);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrWarehouseNotFound));
        }

        [Test]
        public async Task ReturnsFailure_WhenInvoiceNotFound()
        {
            // Arrange
            importInvoiceService
                .Setup(s => s.GetInvoiceByNumberAsync(warehouseId, invoice.InvoiceNumber))
                .ReturnsAsync(OperationResult<ImportInvoice>.Failure("Invoice not found"));

            // Act
            var result = await exportDataService.GetAvailableProductsForInvoiceAsync(warehouseId, userId, invoice.InvoiceNumber);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invoice not found"));
        }

        [Test]
        public async Task ReturnsProducts_WhenStockAvailable()
        {
            // Arrange
            importInvoiceService
                .Setup(s => s.GetInvoiceByNumberAsync(warehouseId, invoice.InvoiceNumber))
                .ReturnsAsync(OperationResult<ImportInvoice>.Ok(invoice));

            var details = invoice.ImportInvoicesDetails.ToList();

            stockService
                .Setup(s => s.GetAvailableQuantityAsync(details[0].Id, It.IsAny<Guid?>()))
                .ReturnsAsync(OperationResult<int>.Ok(3));

            stockService
                .Setup(s => s.GetAvailableQuantityAsync(details[1].Id, It.IsAny<Guid?>()))
                .ReturnsAsync(OperationResult<int>.Ok(5));

            // Act
            var result = await exportDataService.GetAvailableProductsForInvoiceAsync(warehouseId, userId, invoice.InvoiceNumber);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Count(), Is.EqualTo(2));

            var products = result.Data.ToList();

            for (int i = 0; i < products.Count; i++)
            {
                Assert.That(products[i].Name, Is.EqualTo(details[i].Product.Name));
                Assert.That(products[i].Category, Is.EqualTo(details[i].Product.Category.Name));

                var expectedAvailableQuantity = i == 0 ? 3 : 5;
                Assert.That(products[i].AvailableQuantity, Is.EqualTo(expectedAvailableQuantity));
            }
        }

        [Test]
        public async Task ReturnsEmptyList_WhenAllStockIsZero()
        {
            // Arrange
            importInvoiceService
                .Setup(s => s.GetInvoiceByNumberAsync(warehouseId, invoice.InvoiceNumber))
                .ReturnsAsync(OperationResult<ImportInvoice>.Ok(invoice));

            foreach (var detail in invoice.ImportInvoicesDetails)
            {
                stockService
                    .Setup(s => s.GetAvailableQuantityAsync(detail.Id, It.IsAny<Guid?>()))
                    .ReturnsAsync(OperationResult<int>.Ok(0));
            }

            // Act
            var result = await exportDataService.GetAvailableProductsForInvoiceAsync(warehouseId, userId, invoice.InvoiceNumber);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data, Is.Empty);
        }
    }
}
