using Moq;
using MockQueryable;
using System.Linq.Expressions;

using WarehouseApp.Web.ViewModels.ExportInvoice;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoice;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoiceDetail;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

namespace WarehouseApp.Services.Tests.ExportInvoiceServiceTests
{
    [TestFixture]
    public class CreateExportInvoiceAsyncTests : ExportInvoiceServiceBaseTests
    {
        private CreateExportInvoiceInputModel inputModel = null!;

        private ImportInvoice importInvoice = null!;

        [SetUp]
        public void TestSetup()
        {
            importInvoice = invoices[0];
            var importInvoiceProduct1 = importInvoice.ImportInvoicesDetails.ElementAt(0);
            var importInvoiceProduct2 = importInvoice.ImportInvoicesDetails.ElementAt(1);
            importInvoiceProduct1.ExportInvoicesPerProduct.Clear();
            importInvoiceProduct2.ExportInvoicesPerProduct.Clear();

            var clientName = "ClientName";
            var clientAddress = "ClientAddress";
            var clientPhone = "0888888888";
            var clientEmail = "email@test.com";
            var date = DateTime.UtcNow;

            var client = new Client
            {
                Id = Guid.NewGuid(),
                Name = clientName,
                Address = clientAddress,
                PhoneNumber = clientPhone,
                Email = clientEmail
            };

            var exportedProducts = new List<CreateExportInvoiceDetailInputModel>()
            {
                new CreateExportInvoiceDetailInputModel()
                {
                    CategoryName = importInvoiceProduct1.Product.Category.Name,
                    ProductName = importInvoiceProduct1.Product.Name,
                    ImportInvoiceNumber = importInvoice.InvoiceNumber,
                    Quantity = 100,
                    UnitPrice = 15m,
                },
                new CreateExportInvoiceDetailInputModel()
                {
                    CategoryName = importInvoiceProduct2.Product.Category.Name,
                    ProductName = importInvoiceProduct2.Product.Name,
                    ImportInvoiceNumber = importInvoice.InvoiceNumber,
                    Quantity = 199,
                    UnitPrice = 20m,
                }
            };

            inputModel = new CreateExportInvoiceInputModel
            {
                WarehouseId = warehouseId,
                ClientName = clientName,
                ClientAddress = clientAddress,
                ClientPhoneNumber = clientPhone,
                ClientEmail = clientEmail,
                Date = date,
                InvoiceNumber = "EXP-003",
                ExportedProducts = exportedProducts
            };

            exportInvoiceRepo.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<ExportInvoice, bool>>>()))
                .ReturnsAsync(false);

            clientService.Setup(x => x.GetOrCreateOrUpdateClientAsync(
                    clientName, clientAddress, clientPhone, clientEmail))
                .ReturnsAsync(OperationResult<Client>.Ok(client));

            exportInvoiceRepo.Setup(x => x.AddAsync(It.IsAny<ExportInvoice>()))
                .Returns(Task.CompletedTask);

            importInvoiceRepo.Setup(x => x.All())
                .Returns(new List<ImportInvoice> { importInvoice }.AsQueryable().BuildMock());

            stockService
                .Setup(x => x.GetAvailableQuantityAsync(importInvoice.ImportInvoicesDetails.ElementAt(0).Id, It.IsAny<Guid?>()))
                .ReturnsAsync(OperationResult<int>.Ok(100));

            stockService
                .Setup(x => x.GetAvailableQuantityAsync(importInvoice.ImportInvoicesDetails.ElementAt(1).Id, It.IsAny<Guid?>()))
                .ReturnsAsync(OperationResult<int>.Ok(200));

            exportInvoiceDetailRepo.Setup(x => x.AddAsync(It.IsAny<ExportInvoiceDetail>()))
                .Returns(Task.CompletedTask);

            exportInvoiceRepo.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);
        }

        [Test]
        public async Task ReturnsFailureUserNotFound_WhenUserNotFound()
        {
            // Arrange
            SetupUserNotFound();

            // Act
            var result = await exportInvoiceService.CreateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task ReturnsFailureNoPermissionOrWarehouseNotFound_WhenWarehouseNotFound()
        {
            // Arrange
            SetupWarehouseNotFound();

            // Act
            var result = await exportInvoiceService.CreateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrWarehouseNotFound));
        }

        [Test]
        public async Task ReturnsFailureDuplicateInvoice_WhenInvoiceAlreadyExists()
        {
            // Arrange
            exportInvoiceRepo.Setup(x => x.ExistsAsync(
                It.IsAny<Expression<Func<ExportInvoice, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await exportInvoiceService.CreateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(DuplicateInvoice));
        }

        [Test]
        public async Task ReturnsFailureCannotCreateExportInvoiceWithoutExportProducts_WhenProductsListEmpty()
        {
            // Arrange
            inputModel.ExportedProducts.Clear();

            // Act
            var result = await exportInvoiceService.CreateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(CannotCreateExportInvoiceWithoutExports));
        }

        [Test]
        public async Task ReturnsFailureProductDuplicate_WhenInputContainsDuplicateExportProducts()
        {
            // Arrange
            var importInvoiceProduct = importInvoice.ImportInvoicesDetails.ElementAt(0);
            var duplicateExportProduct = new CreateExportInvoiceDetailInputModel()
            {
                CategoryName = importInvoiceProduct.Product.Category.Name,
                ProductName = importInvoiceProduct.Product.Name,
                ImportInvoiceNumber = importInvoice.InvoiceNumber,
                Quantity = 100,
                UnitPrice = 15m,
            };

            inputModel.ExportedProducts.Add(duplicateExportProduct);

            // Act
            var result = await exportInvoiceService.CreateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ExportDuplicate));
        }

        [Test]
        public async Task ReturnsFailureClientCreationFailure_WhenClientServiceThrows()
        {
            // Arrange
            clientService.Setup(x => x.GetOrCreateOrUpdateClientAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Client creation failed"));

            // Act
            var result = await exportInvoiceService.CreateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.Client.CreationFailure));
        }

        [Test]
        public async Task ReturnsFailure_WhenImportInvoiceNotFound()
        {
            // Arrange
            importInvoiceRepo.Setup(x => x.All())
                .Returns(new List<ImportInvoice>().AsQueryable().BuildMock());

            // Act
            var result = await exportInvoiceService.CreateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrImportInvoiceNotFound));
        }

        [Test]
        public async Task ReturnsFailure_WhenProductInImportInvoiceNotFound()
        {
            // Arrange
            var detailToRemove = importInvoice.ImportInvoicesDetails.ElementAt(0);
            importInvoice.ImportInvoicesDetails.Remove(detailToRemove);

            // Act
            var result = await exportInvoiceService.CreateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ProductNotFoundInImportInvoice));
        }

        [Test]
        public async Task ReturnsFailure_WhenExportInvoiceDateIsBeforeImportInvoiceDate()
        {
            // Arrange
            importInvoice.Date = DateTime.UtcNow;

            // Act
            var result = await exportInvoiceService.CreateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(CannotExportBeforeImportDate));
        }

        [Test]
        public async Task ReturnsFailure_WhenStockIsInsufficient()
        {
            // Arrange
            var product = importInvoice.ImportInvoicesDetails.ElementAt(0);
            product.Quantity = 99;

            stockService
                .Setup(x => x.GetAvailableQuantityAsync(product.Id, It.IsAny<Guid?>()))
                .ReturnsAsync(OperationResult<int>.Ok(99));

            // Act
            var result = await exportInvoiceService.CreateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(InsufficientStock));
        }

        [Test]
        public async Task ReturnsFailure_WhenAddAsyncThrows()
        {
            // Arrange
            exportInvoiceDetailRepo.Setup(x => x.AddAsync(It.IsAny<ExportInvoiceDetail>()))
                .ThrowsAsync(new Exception("add fail"));

            // Act
            var result = await exportInvoiceService.CreateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.ExportInvoiceDetail.CreationFailure));
        }

        [Test]
        public async Task ReturnsFailure_WhenSaveChangesAsyncThrows()
        {
            // Arrange
            exportInvoiceRepo.Setup(x => x.SaveChangesAsync())
                .ThrowsAsync(new Exception("save fail"));

            // Act
            var result = await exportInvoiceService.CreateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.ExportInvoice.CreationFailure));
        }

        [Test]
        public async Task ReturnsSuccessWithCorrectProducts_WhenMultipleProductsAreGiven()
        {
            // Arrange
            var capturedDetails = new List<ExportInvoiceDetail>();

            exportInvoiceDetailRepo.Setup(x => x.AddAsync(It.IsAny<ExportInvoiceDetail>()))
                .Callback<ExportInvoiceDetail>(detail => capturedDetails.Add(detail))
                .Returns(Task.CompletedTask);

            // Act
            var result = await exportInvoiceService.CreateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(capturedDetails.Count, Is.EqualTo(inputModel.ExportedProducts.Count));

            for (int i = 0; i < capturedDetails.Count; i++)
            {
                var expected = inputModel.ExportedProducts[i];
                var actual = capturedDetails[i];

                Assert.That(actual.Quantity, Is.EqualTo(expected.Quantity));
                Assert.That(actual.UnitPrice, Is.EqualTo(expected.UnitPrice ?? importInvoice.ImportInvoicesDetails.ElementAt(i).UnitPrice));
                Assert.That(actual.ImportInvoiceDetailId, Is.EqualTo(importInvoice.ImportInvoicesDetails.ElementAt(i).Id));
            }
        }
    }
}
