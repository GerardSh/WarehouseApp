using Moq;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

using WarehouseApp.Web.ViewModels.ImportInvoice;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoice;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoiceDetail;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

namespace WarehouseApp.Services.Tests.ImportInvoiceServiceTests
{
    [TestFixture]
    public class CreateImportInvoiceAsyncTests : ImportInvoiceServiceBaseTests
    {
        private CreateImportInvoiceInputModel inputModel;

        private Guid categoryId;

        [SetUp]
        public void TestSetup()
        {
            inputModel = new CreateImportInvoiceInputModel
            {
                WarehouseId = warehouseId,
                InvoiceNumber = "INV-123",
                Products = new List<CreateImportInvoiceDetailInputModel>
                    {
                        new CreateImportInvoiceDetailInputModel { ProductName = "Prod1", CategoryName = "Cat1" }
                    },
                SupplierName = "SupplierX",
                SupplierAddress = "AddressX",
                SupplierPhoneNumber = "12345",
                SupplierEmail = "email@example.com"
            };

            clientService.Setup(x => x.GetOrCreateOrUpdateClientAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(OperationResult<Client>.Ok(new Client
                {
                    Id = Guid.NewGuid(),
                    Name = "ClientName",
                    Address = "ClientAddress",
                    PhoneNumber = "12345",
                    Email = "email@test.com"
                }));

            categoryId = Guid.NewGuid();

            categoryService
                .Setup(x => x.GetOrCreateOrUpdateCategoryAsync(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(OperationResult<Category>.Ok(
                    new Category
                    {
                        Id = categoryId,
                        Name = "CategoryName",
                        Description = "CategoryDescription"
                    }));


            productService
                .Setup(x => x.GetOrCreateOrUpdateProductAsync(
                    It.IsAny<string>(), It.IsAny<string>(), categoryId))
                .ReturnsAsync(OperationResult<Product>.Ok(
                    new Product
                    {
                        Id = categoryId,
                        Name = "ProductName",
                        Description = "ProductDescription"
                    }));
        }

        [Test]
        public async Task ReturnsFailureUserNotFound_WhenUserNotFound()
        {
            // Arrange
            SetupUserNotFound();

            // Act
            var result = await importInvoiceService.CreateImportInvoiceAsync(inputModel, userId);

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
            var result = await importInvoiceService.CreateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrWarehouseNotFound));
        }

        [Test]
        public async Task ReturnsFailureDuplicateInvoice_WhenInvoiceAlreadyExists()
        {
            // Arrange
            importInvoiceRepo.Setup(x => x.ExistsAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<ImportInvoice, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await importInvoiceService.CreateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(DuplicateInvoice));
        }

        [Test]
        public async Task ReturnsFailureCannotCreateInvoiceWithoutProducts_WhenProductsListEmpty()
        {
            // Arrange
            importInvoiceRepo.Setup(x => x.ExistsAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<ImportInvoice, bool>>>()))
                .ReturnsAsync(false);

            inputModel.Products.Clear();

            // Act
            var result = await importInvoiceService.CreateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(CannotCreateInvoiceWithoutProducts));
        }

        [Test]
        public async Task ReturnsFailureProductDuplicate_WhenInputContainsDuplicateProducts()
        {
            // Arrange
            importInvoiceRepo.Setup(x => x.ExistsAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<ImportInvoice, bool>>>()))
                .ReturnsAsync(false);

            inputModel.Products = new List<CreateImportInvoiceDetailInputModel>
                {
                    new CreateImportInvoiceDetailInputModel { ProductName = "Prod1", CategoryName = "Cat1" },
                    new CreateImportInvoiceDetailInputModel { ProductName = "prod1", CategoryName = "cat1" }
                };

            // Act
            var result = await importInvoiceService.CreateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ProductDuplicate));
        }

        [Test]
        public async Task ReturnsFailureClientCreationFailure_WhenClientServiceThrows()
        {
            // Arrange
            importInvoiceRepo.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ImportInvoice, bool>>>()))
                .ReturnsAsync(false);

            clientService.Setup(x => x.GetOrCreateOrUpdateClientAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Client creation failed"));

            // Act
            var result = await importInvoiceService.CreateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.Client.CreationFailure));

            logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains(ErrorMessages.Client.CreationFailure)),
                It.Is<Exception>(ex => ex.Message == "Client creation failed"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task ReturnsFailureCategoryCreationFailure_WhenCategoryServiceThrows()
        {
            // Arrange
            categoryService.Setup(x => x.GetOrCreateOrUpdateCategoryAsync(
                It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("category fail"));

            // Act
            var result = await importInvoiceService.CreateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.Category.CreationFailure));

            logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains(ErrorMessages.Category.CreationFailure)),
                It.Is<Exception>(ex => ex.Message == "category fail"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task ReturnsFailureProductCreationFailure_WhenProductServiceThrows()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            productService.Setup(x => x.GetOrCreateOrUpdateProductAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("product fail"));

            // Act
            var result = await importInvoiceService.CreateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.Product.CreationFailure));

            logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains(ErrorMessages.Product.CreationFailure)),
                It.Is<Exception>(ex => ex.Message == "product fail"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task ReturnsFailureDetailCreationFailure_WhenDetailRepoThrows()
        {
            // Arrange
            importInvoiceDetailRepo.Setup(x => x.AddAsync(It.IsAny<ImportInvoiceDetail>()))
                .ThrowsAsync(new Exception("detail fail"));

            // Act
            var result = await importInvoiceService.CreateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.ImportInvoiceDetail.CreationFailure));

            logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains(ErrorMessages.ImportInvoiceDetail.CreationFailure)),
                It.Is<Exception>(ex => ex.Message == "detail fail"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task ReturnsFailureInvoiceCreationFailure_WhenExistsAsyncThrows()
        {
            // Arrange
            importInvoiceRepo.Setup(x => x.ExistsAsync(
                    It.IsAny<Expression<Func<ImportInvoice, bool>>>()))
                .ThrowsAsync(new Exception("exists check fail"));

            // Act
            var result = await importInvoiceService.CreateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.ImportInvoice.CreationFailure));

            logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(ErrorMessages.ImportInvoice.CreationFailure)),
                It.Is<Exception>(ex => ex.Message == "exists check fail"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Test]
        public async Task ReturnsSuccess_WhenInvoiceCreatedSuccessfully()
        {
            // Act
            var result = await importInvoiceService.CreateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task ReturnsSuccessWithCorrectProducts_WhenMultipleProductsAreGiven()
        {
            // Arrange
            inputModel.Products = new List<CreateImportInvoiceDetailInputModel>
                {
                    new CreateImportInvoiceDetailInputModel { ProductName = "ProdA", ProductDescription = "DescA", CategoryName = "CatA" },
                    new CreateImportInvoiceDetailInputModel { ProductName = "ProdB", ProductDescription = "DescB", CategoryName = "CatB" }
                };

            var productA = new Product { Id = Guid.NewGuid(), Name = "ProdA", Description = "DescA" };
            var productB = new Product { Id = Guid.NewGuid(), Name = "ProdB", Description = "DescB" };

            // Setup the category service to return unique category Ids
            var catAId = Guid.NewGuid();
            var catBId = Guid.NewGuid();
            categoryService.Setup(x => x.GetOrCreateOrUpdateCategoryAsync("CatA", null))
                .ReturnsAsync(OperationResult<Category>.Ok(new Category { Id = catAId, Name = "CatA" }));
            categoryService.Setup(x => x.GetOrCreateOrUpdateCategoryAsync("CatB", null))
                .ReturnsAsync(OperationResult<Category>.Ok(new Category { Id = catBId, Name = "CatB" }));

            // Setup product service to return the correct product based on input
            productService.Setup(x => x.GetOrCreateOrUpdateProductAsync("ProdA", "DescA", catAId))
                .ReturnsAsync(OperationResult<Product>.Ok(productA));
            productService.Setup(x => x.GetOrCreateOrUpdateProductAsync("ProdB", "DescB", catBId))
                .ReturnsAsync(OperationResult<Product>.Ok(productB));

            var capturedDetails = new List<ImportInvoiceDetail>();

            importInvoiceDetailRepo.Setup(x => x.AddAsync(It.IsAny<ImportInvoiceDetail>()))
                .Callback<ImportInvoiceDetail>(detail => capturedDetails.Add(detail))
                .Returns(Task.CompletedTask);

            // Act
            var result = await importInvoiceService.CreateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(capturedDetails.Count, Is.EqualTo(2));
        }
    }
}
