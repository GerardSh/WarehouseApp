using Moq;
using MockQueryable;
using System.Linq.Expressions;

using WarehouseApp.Web.ViewModels.ImportInvoice;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoice;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoiceDetail;

namespace WarehouseApp.Services.Tests.ImportInvoiceServiceTests
{
    [TestFixture]
    public class UpdateImportInvoiceAsyncTests : ImportInvoiceServiceBaseTests
    {
        private EditImportInvoiceInputModel inputModel;
        private ImportInvoice importInvoice;
        private ExportInvoice exportInvoice;
        private ImportInvoiceDetail importDetail;
        private ImportInvoiceDetail importDetail2;
        private string invoiceNumber = "INV-123";
        private Guid invoiceId = Guid.NewGuid();
        private Guid detailId = Guid.NewGuid();
        private Guid detailId2 = Guid.NewGuid();
        private Guid categoryId;
        private Guid clientId = Guid.NewGuid();
        private Guid productId = Guid.NewGuid();
        private Guid productId2 = Guid.NewGuid();
        private Guid exportInvoiceId = Guid.NewGuid();

        [SetUp]
        public void TestSetup()
        {
            var now = DateTime.UtcNow;

            var client = new Client
            {
                Id = clientId,
                Name = "Client Ltd.",
                Address = "Sofia, Bulgaria",
                PhoneNumber = "0888123456",
                Email = "client@example.com"
            };

            var category = new Category
            {
                Id = categoryId,
                Name = "Fruits",
                Description = "Edible sweet items"
            };

            var product = new Product
            {
                Id = productId,
                Name = "Apple",
                Description = "Fresh red apple",
                CategoryId = categoryId,
                Category = category
            };

            var product2 = new Product
            {
                Id = productId2,
                Name = "Watermelon",
                Description = "Fresh watermelon",
                CategoryId = categoryId,
                Category = category
            };

            var warehouse = new Warehouse
            {
                Id = warehouseId,
                Name = "Main Warehouse",
                Address = "Plovdiv"
            };

            importInvoice = new ImportInvoice
            {
                Id = invoiceId,
                InvoiceNumber = invoiceNumber,
                Date = now,
                SupplierId = clientId,
                Supplier = client,
                WarehouseId = warehouseId,
                Warehouse = warehouse,
                ImportInvoicesDetails = new List<ImportInvoiceDetail>()
            };

            importDetail = new ImportInvoiceDetail
            {
                Id = detailId,
                ImportInvoiceId = invoiceId,
                ImportInvoice = importInvoice,
                ProductId = productId,
                Product = product,
                Quantity = 10,
                UnitPrice = 5.5m
            };

            importInvoice.ImportInvoicesDetails.Add(importDetail);

            // Details 2 is needed for some tests only
            importDetail2 = new ImportInvoiceDetail
            {
                Id = detailId2,
                ImportInvoiceId = invoiceId,
                ImportInvoice = importInvoice,
                Product = product2,
                Quantity = 10,
                UnitPrice = 5.5m,
                ExportInvoicesPerProduct = new List<ExportInvoiceDetail>()
            };

            exportInvoice = new ExportInvoice
            {
                Id = exportInvoiceId,
                InvoiceNumber = "EXP-001",
                Date = now,
                ClientId = clientId,
                Client = new Client
                {
                    Id = clientId,
                    Name = "Export Client",
                    Address = "Varna",
                    PhoneNumber = "0899123456",
                    Email = "export@example.com"
                },
                WarehouseId = warehouseId,
                Warehouse = warehouse,
                ExportInvoicesDetails = new List<ExportInvoiceDetail>()
            };

            var exportDetail = new ExportInvoiceDetail
            {
                Id = Guid.NewGuid(),
                ExportInvoiceId = exportInvoiceId,
                ExportInvoice = exportInvoice,
                ImportInvoiceDetailId = detailId,
                ImportInvoiceDetail = importDetail,
                Quantity = 5,
                UnitPrice = 6.5m
            };

            exportInvoice.ExportInvoicesDetails.Add(exportDetail);

            inputModel = new EditImportInvoiceInputModel
            {
                Id = invoiceId,
                WarehouseId = warehouseId,
                InvoiceNumber = invoiceNumber,
                Date = now,
                SupplierName = client.Name,
                SupplierAddress = client.Address,
                SupplierPhoneNumber = client.PhoneNumber,
                SupplierEmail = client.Email,
                Products = new List<EditImportInvoiceDetailInputModel>
                {
                    new EditImportInvoiceDetailInputModel
                    {
                        Id = detailId,
                        ProductName = product.Name,
                        CategoryName = category.Name,
                        Quantity = 10,
                        UnitPrice = 5.5m
                    }
                }
            };

            importInvoiceRepo.Setup(r => r.AllTracked())
                .Returns(new List<ImportInvoice> { importInvoice }.AsQueryable().BuildMock());

            importInvoiceRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<ImportInvoice, bool>>>()))
                .ReturnsAsync(false);

            clientService.Setup(x => x.GetOrCreateOrUpdateClientAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(OperationResult<Client>.Ok(client));

            importInvoiceDetailRepo.Setup(r => r.All())
                .Returns(importInvoice.ImportInvoicesDetails.AsQueryable().BuildMock());

            importInvoiceDetailRepo.Setup(r => r.AllTracked())
                .Returns(importInvoice.ImportInvoicesDetails.AsQueryable().BuildMock());

            exportInvoiceDetailRepo.Setup(r => r.All())
                .Returns(new List<ExportInvoiceDetail> { exportDetail }.AsQueryable().BuildMock());

            categoryService.Setup(x => x.GetOrCreateOrUpdateCategoryAsync(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(OperationResult<Category>.Ok(category));

            productService.Setup(x => x.GetOrCreateOrUpdateProductAsync(
                    It.IsAny<string>(), It.IsAny<string>(), categoryId))
                .ReturnsAsync(OperationResult<Product>.Ok(product));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            SetupUserNotFound();

            // Act
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

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
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrWarehouseNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenInvoiceNotFound()
        {
            // Arrange
            importInvoiceRepo.Setup(r => r.AllTracked())
                .Returns(new List<ImportInvoice>().AsQueryable().BuildMock());

            // Act
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrImportInvoiceNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenInvoiceNumberAlreadyExists()
        {
            // Arrange
            importInvoiceRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<ImportInvoice, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(DuplicateInvoice));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenProductListIsEmpty()
        {
            // Arrange
            inputModel.Products.Clear();

            importInvoiceRepo.Setup(r => r.AllTracked())
                .Returns(new List<ImportInvoice>
                {
                   new ImportInvoice
                       {
                          Id = inputModel.Id,
                          WarehouseId = inputModel.WarehouseId,
                          InvoiceNumber = inputModel.InvoiceNumber
                       }
                }.AsQueryable().BuildMock());

            // Act
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(CannnotDeleteAllProducts));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenDuplicateProductsProvided()
        {
            // Arrange
            inputModel.Products = new List<EditImportInvoiceDetailInputModel>
            {
                new EditImportInvoiceDetailInputModel
                {
                    ProductName = "ProductA",
                    CategoryName = "Category1"
                },
                new EditImportInvoiceDetailInputModel
                {
                    ProductName = "ProductA",
                    CategoryName = "Category1"
                }
            };

            importInvoiceRepo.Setup(r => r.AllTracked())
                .Returns(new List<ImportInvoice>
                {
                     new ImportInvoice
                     {
                         Id = inputModel.Id,
                         WarehouseId = inputModel.WarehouseId,
                         InvoiceNumber = inputModel.InvoiceNumber
                     }
                }.AsQueryable().BuildMock());

            // Act
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ProductDuplicate));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenClientServiceThrows()
        {
            // Arrange
            clientService.Setup(c => c.GetOrCreateOrUpdateClientAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Client creation failed"));

            // Act
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.Client.CreationFailure));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenNewImportDateIsAfterExportDate()
        {
            // Arrange
            exportInvoice.Date = inputModel.Date.AddDays(-1);

            // Act
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Does.StartWith(InvalidDate));
            Assert.That(result.ErrorMessage, Does.Contain("Apple"));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenCategoryServiceThrows()
        {
            // Arrange
            categoryService.Setup(x => x.GetOrCreateOrUpdateCategoryAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.Category.CreationFailure));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenProductServiceThrows()
        {
            // Arrange
            productService.Setup(x => x.GetOrCreateOrUpdateProductAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.Product.CreationFailure));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExistingDetailNotFound()
        {
            // Arrange
            importInvoiceDetailRepo.Setup(r => r.AllTracked())
                .Returns(new List<ImportInvoiceDetail>().AsQueryable().BuildMock());

            // Act
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ProductNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenQuantityIsLowerThanExported()
        {
            // Arrange
            inputModel.Products[0].Quantity = 2;

            // Act
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Does.StartWith("Cannot set quantity"));
            Assert.That(result.ErrorMessage, Does.Contain("Apple"));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenAddNewDetailThrows()
        {
            // Arrange
            inputModel.Products[0].Id = null;

            importInvoiceDetailRepo.Setup(r => r.AddAsync(It.IsAny<ImportInvoiceDetail>()))
                .ThrowsAsync(new Exception("Insert failed"));

            // Act
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.ImportInvoiceDetail.CreationFailure));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenTryingToDeleteDetailBoundToExport()
        {
            // Arrange
            importDetail2.ExportInvoicesPerProduct.Add(new ExportInvoiceDetail
            {
                ExportInvoice = exportInvoice,
                Quantity = 5,
                Id = Guid.NewGuid()
            });

            importInvoice.ImportInvoicesDetails.Add(importDetail2);

            // Act
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ProductDeletionFailure));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExceptionOccursDuringDetailRemoval()
        {
            // Arrange
            importInvoice.ImportInvoicesDetails.Add(importDetail2);

            importInvoiceDetailRepo.Setup(r => r.DeleteRange(It.IsAny<IEnumerable<ImportInvoiceDetail>>()))
                .Throws(new Exception("Simulated failure"));

            // Act
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.ImportInvoiceDetail.DeletionFailure));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenSaveChangesThrowsException()
        {
            // Arrange
            importInvoiceRepo.Setup(r => r.SaveChangesAsync())
                .Throws(new Exception("Database error"));

            // Act
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.ImportInvoice.EditingFailure));
        }

        [Test]
        public async Task ShouldUpdateImportInvoiceSuccessfully_WhenInputIsValid()
        {
            // Arrange
            importInvoice.ImportInvoicesDetails.Add(importDetail2);

            ImportInvoice? savedInvoice = null;

            importInvoiceRepo.Setup(r => r.SaveChangesAsync())
                .Callback(() =>
                {
                    savedInvoice = importInvoice;
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);

            Assert.That(savedInvoice, Is.Not.Null);
            Assert.That(savedInvoice!.ImportInvoicesDetails.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task ShouldUpdateImportInvoiceSuccessfully_WhenAllFieldsAndDetailsAreModified()
        {
            // Arrange – Update Invoice Fields
            inputModel.InvoiceNumber = "Inv002";
            inputModel.SupplierName = "Client2";
            inputModel.SupplierAddress = "Address2";
            inputModel.Date = DateTime.Now.AddDays(-2);
            inputModel.SupplierEmail = "test@email.com";

            var updatedClient = new Client
            {
                Name = inputModel.SupplierName,
                Address = inputModel.SupplierAddress,
                PhoneNumber = inputModel.SupplierPhoneNumber,
                Email = inputModel.SupplierEmail
            };

            clientService.Setup(c => c.GetOrCreateOrUpdateClientAsync(
                    inputModel.SupplierName,
                    inputModel.SupplierAddress,
                    inputModel.SupplierPhoneNumber,
                    inputModel.SupplierEmail))
                .ReturnsAsync(OperationResult<Client>.Ok(updatedClient));

            // Modify Existing Product Detail
            var existingDetail = importDetail;
            var existingProductInput = inputModel.Products[0];
            existingProductInput.Id = existingDetail.Id;
            existingProductInput.ProductName = "New Product";
            existingProductInput.ProductDescription = "Updated description";
            existingProductInput.CategoryName = "New Category";
            existingProductInput.CategoryDescription = "Updated category";
            existingProductInput.Quantity = 20;
            existingProductInput.UnitPrice = 10m;

            // Add New Product Detail
            var newProductInput = new EditImportInvoiceDetailInputModel
            {
                Id = null,
                ProductName = "New Product2",
                ProductDescription = "Second product",
                CategoryName = "New Category2",
                CategoryDescription = "Second category",
                Quantity = 10,
                UnitPrice = 5.5m,
            };
            inputModel.Products.Add(newProductInput);

            // Prepare Categories
            var newCategory1 = new Category { Id = Guid.NewGuid(), Name = "New Category" };
            var newCategory2 = new Category { Id = Guid.NewGuid(), Name = "New Category2" };

            categoryService.SetupSequence(c => c.GetOrCreateOrUpdateCategoryAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(OperationResult<Category>.Ok(newCategory1))
                .ReturnsAsync(OperationResult<Category>.Ok(newCategory2));

            // Prepare Products
            var newProduct1 = new Product { Id = Guid.NewGuid(), Name = "New Product", Category = newCategory1 };
            var newProduct2 = new Product { Id = Guid.NewGuid(), Name = "New Product2", Category = newCategory2 };

            productService.SetupSequence(p => p.GetOrCreateOrUpdateProductAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(OperationResult<Product>.Ok(newProduct1))
                .ReturnsAsync(OperationResult<Product>.Ok(newProduct2));

            // Simulate existing ImportInvoiceDetail
            importInvoiceDetailRepo.Setup(r => r.AllTracked())
                .Returns(new List<ImportInvoiceDetail> { existingDetail }
                    .AsQueryable()
                    .BuildMock());

            // Simulate no exports
            exportInvoiceDetailRepo.Setup(r => r.All())
                .Returns(new List<ExportInvoiceDetail>().AsQueryable().BuildMock());

            // Capture Saved Invoice
            ImportInvoice? savedInvoice = null;

            importInvoiceRepo.Setup(r => r.SaveChangesAsync())
                .Callback(() => savedInvoice = importInvoice)
                .Returns(Task.CompletedTask);

            importInvoiceDetailRepo
                .Setup(r => r.AddAsync(It.IsAny<ImportInvoiceDetail>()))
                .Callback<ImportInvoiceDetail>(detail =>
                {
                    importInvoice.ImportInvoicesDetails.Add(detail);
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await importInvoiceService.UpdateImportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);

            Assert.That(savedInvoice, Is.Not.Null);
            Assert.That(savedInvoice!.ImportInvoicesDetails.Count, Is.EqualTo(2));

            var updatedDetail = savedInvoice.ImportInvoicesDetails.FirstOrDefault(d => d.Id == existingDetail.Id);
            Assert.That(updatedDetail, Is.Not.Null);
            Assert.That(updatedDetail!.Quantity, Is.EqualTo(20));
            Assert.That(updatedDetail.UnitPrice, Is.EqualTo(10m));
            Assert.That(updatedDetail.Product.Name, Is.EqualTo("New Product"));
            Assert.That(updatedDetail.Product.Category.Name, Is.EqualTo("New Category"));
        }
    }
}