using Moq;

using WarehouseApp.Data.Models;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoice;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

namespace WarehouseApp.Services.Tests.ImportInvoiceServiceTests
{
    [TestFixture]
    public class GetImportInvoiceDetailsAsyncTests : ImportInvoiceServiceBaseTests
    {
        private Guid invoiceId;
        private ImportInvoice invoice;

        [SetUp]
        public void SetupTest()
        {
            invoiceId = Guid.NewGuid();

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Electronics",
                Description = "Electronic items"
            };

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Laptop",
                Description = "Gaming laptop",
                Category = category
            };

            invoice = new ImportInvoice
            {
                Id = invoiceId,
                InvoiceNumber = "INV-123",
                Date = new DateTime(2025, 7, 30),
                Supplier = new Client
                {
                    Name = "Supplier X",
                    Address = "Supplier Address",
                    PhoneNumber = "123456789",
                    Email = "supplier@example.com"
                },
                ImportInvoicesDetails = new List<ImportInvoiceDetail>
                {
                    new ImportInvoiceDetail
                    {
                        Product = product,
                        Quantity = 5,
                        UnitPrice = 1000m
                    }
                }
            };

            importInvoiceRepo.Setup(r => r.GetInvoiceWithDetailsAsync(invoiceId, warehouseId))
                .ReturnsAsync(invoice);
        }

        [Test]
        public async Task ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            SetupUserNotFound();

            // Act
            var result = await importInvoiceService.GetImportInvoiceDetailsAsync(warehouseId, invoiceId, userId);

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
            var result = await importInvoiceService.GetImportInvoiceDetailsAsync(warehouseId, invoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrWarehouseNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenInvoiceNotFound()
        {
            // Arrange
            importInvoiceRepo.Setup(r => r.GetInvoiceWithDetailsAsync(invoiceId, warehouseId))
                .ReturnsAsync((ImportInvoice?)null);

            // Act
            var result = await importInvoiceService.GetImportInvoiceDetailsAsync(warehouseId, invoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrImportInvoiceNotFound));
        }

        [Test]
        public async Task ShouldReturnSuccessWithCorrectViewModel_WhenValidDataProvided()
        {
            // Act
            var result = await importInvoiceService.GetImportInvoiceDetailsAsync(warehouseId, invoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            var vm = result.Data!;
            Assert.That(vm.Id, Is.EqualTo(invoice.Id));
            Assert.That(vm.InvoiceNumber, Is.EqualTo(invoice.InvoiceNumber));
            Assert.That(vm.Date, Is.EqualTo(invoice.Date));
            Assert.That(vm.SupplierName, Is.EqualTo(invoice.Supplier.Name));
            Assert.That(vm.SupplierAddress, Is.EqualTo(invoice.Supplier.Address));
            Assert.That(vm.SupplierPhone, Is.EqualTo(invoice.Supplier.PhoneNumber));
            Assert.That(vm.SupplierEmail, Is.EqualTo(invoice.Supplier.Email));

            Assert.That(vm.Products.Count, Is.EqualTo(invoice.ImportInvoicesDetails.Count));

            var detailVm = vm.Products[0];
            var detail = invoice.ImportInvoicesDetails.ElementAt(0);

            Assert.That(detailVm.ProductName, Is.EqualTo(detail.Product.Name));
            Assert.That(detailVm.ProductDescription, Is.EqualTo(detail.Product.Description));
            Assert.That(detailVm.CategoryName, Is.EqualTo(detail.Product.Category.Name));
            Assert.That(detailVm.CategoryDescription, Is.EqualTo(detail.Product.Category.Description));
            Assert.That(detailVm.Quantity, Is.EqualTo(detail.Quantity));
            Assert.That(detailVm.UnitPrice, Is.EqualTo(detail.UnitPrice ?? 0));
            Assert.That(detailVm.Total, Is.EqualTo(detail.Quantity * (detail.UnitPrice ?? 0)));
        }

        [Test]
        public async Task ShouldReturnNA_WhenSupplierPhoneAndEmailAreNull()
        {
            // Arrange
            invoice.Supplier.PhoneNumber = null;
            invoice.Supplier.Email = null;

            // Act
            var result = await importInvoiceService.GetImportInvoiceDetailsAsync(warehouseId, invoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            Assert.That(result.Data!.SupplierPhone, Is.EqualTo("N/A"));
            Assert.That(result.Data!.SupplierEmail, Is.EqualTo("N/A"));
        }

        [Test]
        public async Task ShouldReturnAllDetailsCorrectly_WhenMultipleProductsExist()
        {
            // Arrange
            var category1 = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Electronics",
                Description = "Electronic items"
            };

            var category2 = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Furniture",
                Description = "Home furniture"
            };

            var product1 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Laptop",
                Description = "Gaming laptop",
                Category = category1
            };

            var product2 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Chair",
                Description = "Office chair",
                Category = category2
            };

            invoice.ImportInvoicesDetails = new List<ImportInvoiceDetail>
            {
                new ImportInvoiceDetail
                {
                    Product = product1,
                    Quantity = 5,
                    UnitPrice = 1000m
                },
                new ImportInvoiceDetail
                {
                    Product = product2,
                    Quantity = 3,
                    UnitPrice = 150m
                }
            };

            importInvoiceRepo.Setup(r => r.GetInvoiceWithDetailsAsync(invoiceId, warehouseId))
                .ReturnsAsync(invoice);

            // Act
            var result = await importInvoiceService.GetImportInvoiceDetailsAsync(warehouseId, invoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.Products.Count, Is.EqualTo(2));

            var detail1 = result.Data!.Products[0];
            var detail2 = result.Data!.Products[1];

            Assert.That(detail1.ProductName, Is.EqualTo("Laptop"));
            Assert.That(detail1.CategoryName, Is.EqualTo("Electronics"));
            Assert.That(detail1.Total, Is.EqualTo(5000m));

            Assert.That(detail2.ProductName, Is.EqualTo("Chair"));
            Assert.That(detail2.CategoryName, Is.EqualTo("Furniture"));
            Assert.That(detail2.Total, Is.EqualTo(450m));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            importInvoiceRepo.Setup(r => r.GetInvoiceWithDetailsAsync(invoiceId, warehouseId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await importInvoiceService.GetImportInvoiceDetailsAsync(warehouseId, invoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.ImportInvoice.GetModelFailure));
        }
    }
}