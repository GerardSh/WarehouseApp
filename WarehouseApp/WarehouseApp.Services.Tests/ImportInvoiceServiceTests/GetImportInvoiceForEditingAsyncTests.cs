using Moq;

using WarehouseApp.Data.Models;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoice;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

namespace WarehouseApp.Services.Tests.ImportInvoiceServiceTests
{
    [TestFixture]
    public class GetImportInvoiceForEditingAsyncTests : ImportInvoiceServiceBaseTests
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
            userManager.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await importInvoiceService.GetImportInvoiceForEditingAsync(warehouseId, invoiceId, userId);

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
            var result = await importInvoiceService.GetImportInvoiceForEditingAsync(warehouseId, invoiceId, userId);

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
            var result = await importInvoiceService.GetImportInvoiceForEditingAsync(warehouseId, invoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrImportInvoiceNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            importInvoiceRepo.Setup(r => r.GetInvoiceWithDetailsAsync(invoiceId, warehouseId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await importInvoiceService.GetImportInvoiceForEditingAsync(warehouseId, invoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.ImportInvoice.GetModelFailure));
        }

        [Test]
        public async Task ShouldReturnCorrectModel_WhenValidData()
        {
            // Arrange
            var invoice = this.invoice;

            importInvoiceRepo.Setup(r => r.GetInvoiceWithDetailsAsync(invoiceId, warehouseId))
                .ReturnsAsync(invoice);

            // Act
            var result = await importInvoiceService.GetImportInvoiceForEditingAsync(warehouseId, invoiceId, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            var model = result.Data!;
            Assert.That(model.Id, Is.EqualTo(invoice.Id));
            Assert.That(model.InvoiceNumber, Is.EqualTo(invoice.InvoiceNumber));
            Assert.That(model.Date, Is.EqualTo(invoice.Date));
            Assert.That(model.WarehouseId, Is.EqualTo(invoice.WarehouseId));
            Assert.That(model.SupplierName, Is.EqualTo(invoice.Supplier.Name));
            Assert.That(model.SupplierAddress, Is.EqualTo(invoice.Supplier.Address));
            Assert.That(model.SupplierEmail, Is.EqualTo(invoice.Supplier.Email));
            Assert.That(model.SupplierPhoneNumber, Is.EqualTo(invoice.Supplier.PhoneNumber));

            Assert.That(model.Products.Count, Is.EqualTo(invoice.ImportInvoicesDetails.Count));

            var productVm = model.Products[0];
            var detail = invoice.ImportInvoicesDetails.ElementAt(0);

            Assert.That(productVm.Id, Is.EqualTo(detail.Id));
            Assert.That(productVm.ProductName, Is.EqualTo(detail.Product.Name));
            Assert.That(productVm.ProductDescription, Is.EqualTo(detail.Product.Description));
            Assert.That(productVm.CategoryName, Is.EqualTo(detail.Product.Category.Name));
            Assert.That(productVm.CategoryDescription, Is.EqualTo(detail.Product.Category.Description));
            Assert.That(productVm.Quantity, Is.EqualTo(detail.Quantity));
            Assert.That(productVm.UnitPrice, Is.EqualTo(detail.UnitPrice));
        }
    }
}