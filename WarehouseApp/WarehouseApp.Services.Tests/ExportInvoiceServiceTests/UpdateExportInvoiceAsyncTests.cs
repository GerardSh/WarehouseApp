using Moq;
using MockQueryable;
using System.Linq.Expressions;

using WarehouseApp.Web.ViewModels.ExportInvoice;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoice;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoiceDetail;

namespace WarehouseApp.Services.Tests.ExportInvoiceServiceTests
{
    [TestFixture]
    public class UpdateExportInvoiceAsyncTests : ExportInvoiceServiceBaseTests
    {
        private EditExportInvoiceInputModel inputModel;
        private ExportInvoice exportInvoice;
        private ImportInvoice importInvoice;

        [SetUp]
        public void TestSetup()
        {
            exportInvoice = exportInvoices[0];
            importInvoice = exportInvoices[0].ExportInvoicesDetails.ElementAt(0).ImportInvoiceDetail.ImportInvoice;

            var importInvoiceDetails = new List<ImportInvoiceDetail>()
            {
               importInvoiceDetail1,
               importInvoiceDetail2
            }
            .AsQueryable()
            .BuildMock();

            var exportInvoiceDetails = new List<ExportInvoiceDetail>()
            {
                exportInvoiceDetail1,
                exportInvoiceDetail2
            }
            .AsQueryable()
            .BuildMock();

            inputModel = new EditExportInvoiceInputModel
            {
                Id = exportInvoice.Id,
                WarehouseId = exportInvoice.WarehouseId,
                InvoiceNumber = exportInvoice.InvoiceNumber,
                Date = exportInvoice.Date,
                ClientName = exportInvoice.Client.Name,
                ClientAddress = exportInvoice.Client.Address,
                ClientPhoneNumber = exportInvoice.Client.PhoneNumber,
                ClientEmail = exportInvoice.Client.Email,
                ExportedProducts = new List<EditExportInvoiceDetailInputModel>
                {
                    new EditExportInvoiceDetailInputModel
                    {
                        Id = exportInvoiceDetail1.Id,
                        ImportInvoiceNumber = exportInvoiceDetail1.ImportInvoiceDetail.ImportInvoice.InvoiceNumber,
                        ProductName = exportInvoiceDetail1.ImportInvoiceDetail.Product.Name,
                        CategoryName = exportInvoiceDetail1.ImportInvoiceDetail.Product.Category.Name,
                        Quantity = exportInvoiceDetail1.Quantity,
                        UnitPrice = exportInvoiceDetail1.UnitPrice
                    },
                    new EditExportInvoiceDetailInputModel
                    {
                        Id = exportInvoiceDetail2.Id,
                        ImportInvoiceNumber = exportInvoiceDetail2.ImportInvoiceDetail.ImportInvoice.InvoiceNumber,
                        ProductName = exportInvoiceDetail2.ImportInvoiceDetail.Product.Name,
                        CategoryName = exportInvoiceDetail2.ImportInvoiceDetail.Product.Category.Name,
                        Quantity = exportInvoiceDetail2.Quantity,
                        UnitPrice = exportInvoiceDetail2.UnitPrice
                    }
                }
            };

            exportInvoiceRepo.Setup(r => r.AllTracked())
                .Returns(new[] { exportInvoice }.AsQueryable().BuildMock());

            exportInvoiceRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<ExportInvoice, bool>>>()))
                .ReturnsAsync(false);

            clientService.Setup(x => x.GetOrCreateOrUpdateClientAsync(
               inputModel.ClientName,
               inputModel.ClientAddress,
               inputModel.ClientPhoneNumber,
               inputModel.ClientEmail))
            .ReturnsAsync(OperationResult<Client>.Ok(exportClient));

            importInvoiceDetailRepo
                .Setup(r => r.AllTracked())
                .Returns(importInvoiceDetails);

            stockService
                .Setup(s => s.GetAvailableQuantityAsync(importInvoiceDetail1.Id, exportInvoiceDetail1.Id))
                .ReturnsAsync(OperationResult<int>.Ok(importInvoiceDetail1.Quantity));

            stockService
                .Setup(s => s.GetAvailableQuantityAsync(importInvoiceDetail2.Id, exportInvoiceDetail2.Id))
                .ReturnsAsync(OperationResult<int>.Ok(importInvoiceDetail2.Quantity));

            exportInvoiceDetailRepo
                .Setup(r => r.GetByIdAsync(exportInvoiceDetail1.Id))
                .ReturnsAsync(exportInvoiceDetail1);

            exportInvoiceDetailRepo
                .Setup(r => r.GetByIdAsync(exportInvoiceDetail2.Id))
                .ReturnsAsync(exportInvoiceDetail2);

            exportInvoiceDetailRepo
                .Setup(r => r.AllTracked())
                .Returns(exportInvoiceDetails);
        }

        [Test]
        public async Task ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            SetupUserNotFound();

            // Act
            var result = await exportInvoiceService.UpdateExportInvoiceAsync(inputModel, userId);

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
            var result = await exportInvoiceService.UpdateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrWarehouseNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExportInvoiceNotFound()
        {
            // Arrange
            exportInvoiceRepo.Setup(r => r.AllTracked())
                .Returns(new List<ExportInvoice>().AsQueryable().BuildMock());

            // Act
            var result = await exportInvoiceService.UpdateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermissionOrExportInvoiceNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExportInvoiceExists()
        {
            // Arrange
            exportInvoiceRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<ExportInvoice, bool>>>()))
                    .ReturnsAsync(true);

            // Act
            var result = await exportInvoiceService.UpdateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(DuplicateInvoice));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExportInvoiceHasNoExports()
        {
            // Arrange
            inputModel.ExportedProducts.Clear();

            // Act
            var result = await exportInvoiceService.UpdateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(CannnotDeleteAllExports));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExportInvoiceHasDuplicateExportsProvided()
        {
            // Arrange
            inputModel.ExportedProducts.Add(inputModel.ExportedProducts[0]);

            // Act
            var result = await exportInvoiceService.UpdateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ExportDuplicate));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenClientServiceThrows()
        {
            // Arrange
            clientService.Setup(c => c.GetOrCreateOrUpdateClientAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Client creation failed"));

            // Act
            var result = await exportInvoiceService.UpdateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.Client.CreationFailure));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenImportInvoiceProductNotFound()
        {
            // Arrange
            importInvoiceDetailRepo
                .Setup(r => r.AllTracked())
                .Returns(new List<ImportInvoiceDetail>()
                    .AsQueryable()
                    .BuildMock());

            // Act
            var result = await exportInvoiceService.UpdateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ProductNotFoundInImportInvoice));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenNewExportInvoiceDateIsBeforeProductDetailDate()
        {
            // Arrange
            inputModel.Date = inputModel.Date.AddYears(-3);

            // Act
            var result = await exportInvoiceService.UpdateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Does.StartWith(InvalidDate));
            Assert.That(result.ErrorMessage, Does.Contain(importInvoiceDetail1.Product.Name));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenStockIsUnsufficient()
        {
            // Arrange
            inputModel.ExportedProducts[0].Quantity = 101;

            stockService
                .Setup(s => s.GetAvailableQuantityAsync(importInvoiceDetail1.Id, exportInvoiceDetail1.Id))
                .ReturnsAsync(OperationResult<int>.Ok(importInvoiceDetail1.Quantity - inputModel.ExportedProducts[0].Quantity));

            // Act
            var result = await exportInvoiceService.UpdateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(InsufficientStock));
            exportInvoiceRepo.Verify(r => r.SaveChangesAsync(), Times.Never());
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExportDetailNotFound()
        {
            // Arrange
            var newId = Guid.NewGuid();
            inputModel.ExportedProducts[0].Id = newId;

            exportInvoiceDetailRepo
                .Setup(r => r.GetByIdAsync(It.Is<Guid>(id => id == newId)))
                .ReturnsAsync((ExportInvoiceDetail?)null);

            stockService
                .Setup(s => s.GetAvailableQuantityAsync(importInvoiceDetail1.Id, newId))
                .ReturnsAsync(OperationResult<int>.Ok(importInvoiceDetail1.Quantity));

            // Act
            var result = await exportInvoiceService.UpdateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ExportNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenDeletingADetailFails()
        {
            // Arrange
            inputModel.ExportedProducts.RemoveAt(1);

            exportInvoiceDetailRepo
                .Setup(r => r.DeleteRange(It.IsAny<IEnumerable<ExportInvoiceDetail>>()))
                .Throws(new Exception("Simulated deletion failure"));

            // Act
            var result = await exportInvoiceService.UpdateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.ExportInvoiceDetail.DeletionFailure));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenSaveChangesThrowsException()
        {
            // Arrange
            exportInvoiceRepo.Setup(r => r.SaveChangesAsync())
                .Throws(new Exception("Database error"));

            // Act
            var result = await exportInvoiceService.UpdateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(ErrorMessages.ExportInvoice.EditingFailure));
        }

        [Test]
        public async Task ShouldUpdateExportInvoiceSuccessfully_WhenAllFieldsAndDetailsAreModifiedAndNewDetailAddedDynamically()
        {
            // Arrange
            var category3 = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Category C",
                Description = "Description C"
            };

            var product3 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product C",
                Category = category3,
                CategoryId = category3.Id,
                Description = "Description C"
            };

            var importDetail3Id = Guid.NewGuid();

            var importDetail3 = new ImportInvoiceDetail
            {
                Id = importDetail3Id,
                Product = product3,
                ProductId = product3.Id,
                Quantity = 100,
                UnitPrice = 10.5m,
            };

            importDetail3.ImportInvoice = importInvoice;
            importDetail3.ImportInvoiceId = importInvoice.Id;

            var dynamicExportDetail = new EditExportInvoiceDetailInputModel
            {
                Id = null,
                ImportInvoiceNumber = importInvoice.InvoiceNumber,
                ProductName = importDetail3.Product.Name,
                CategoryName = importDetail3.Product.Category.Name,
                Quantity = 50,
                UnitPrice = 20m
            };

            inputModel.ExportedProducts.Add(dynamicExportDetail);

            inputModel.InvoiceNumber = "UPDATED-INV-1001";
            inputModel.Date = inputModel.Date.AddDays(1);
            inputModel.ClientName = "Updated Client";
            inputModel.ClientAddress = "Updated Address";
            inputModel.ClientPhoneNumber = "555-999";
            inputModel.ClientEmail = "updated@email.com";

            inputModel.ExportedProducts[0].Quantity += 10;
            inputModel.ExportedProducts[0].UnitPrice += 5;

            var importInvoiceDetails = new List<ImportInvoiceDetail>()
            {
               importInvoiceDetail1,
               importInvoiceDetail2,
               importDetail3
            }
            .AsQueryable()
            .BuildMock();

            importInvoiceDetailRepo
                .Setup(r => r.AllTracked())
                .Returns(importInvoiceDetails);

            stockService
                .Setup(s => s.GetAvailableQuantityAsync(importDetail3Id, It.IsAny<Guid?>()))
                .ReturnsAsync(OperationResult<int>.Ok(importDetail3.Quantity));

            ExportInvoice? capturedExportInvoice = null;

            exportInvoiceRepo
                .Setup(r => r.SaveChangesAsync())
                .Callback(() => capturedExportInvoice = exportInvoice)
                .Returns(Task.CompletedTask);

            ExportInvoiceDetail? capturedNewDetail = null;

            exportInvoiceDetailRepo
                .Setup(r => r.AddAsync(It.IsAny<ExportInvoiceDetail>()))
                .Callback<ExportInvoiceDetail>(detail =>
                {
                    capturedNewDetail = detail;
                })
                .Returns(Task.CompletedTask);

            clientService.Setup(x => x.GetOrCreateOrUpdateClientAsync(
                inputModel.ClientName,
                inputModel.ClientAddress,
                inputModel.ClientPhoneNumber,
                inputModel.ClientEmail))
            .ReturnsAsync(OperationResult<Client>.Ok(new Client()
            {
                Name = inputModel.ClientName,
                Address = inputModel.ClientAddress,
                PhoneNumber = inputModel.ClientPhoneNumber,
                Email = inputModel.ClientEmail
            }));

            // Act
            var result = await exportInvoiceService.UpdateExportInvoiceAsync(inputModel, userId);
            capturedExportInvoice!.ExportInvoicesDetails.Add(capturedNewDetail!);

            // Assert
            Assert.That(result.Success, Is.True);

            Assert.That(capturedExportInvoice, Is.Not.Null);
            Assert.That(capturedExportInvoice!.InvoiceNumber, Is.EqualTo(inputModel.InvoiceNumber));
            Assert.That(capturedExportInvoice.Date, Is.EqualTo(inputModel.Date));
            Assert.That(capturedExportInvoice.Client.Name, Is.EqualTo(inputModel.ClientName));
            Assert.That(capturedExportInvoice.Client.Address, Is.EqualTo(inputModel.ClientAddress));
            Assert.That(capturedExportInvoice.Client.PhoneNumber, Is.EqualTo(inputModel.ClientPhoneNumber));
            Assert.That(capturedExportInvoice.Client.Email, Is.EqualTo(inputModel.ClientEmail));

            // Export Details Assertions
            Assert.That(capturedExportInvoice.ExportInvoicesDetails.Count, Is.EqualTo(3));

            // 1. Assert modified detail
            var modifiedDetail = capturedExportInvoice.ExportInvoicesDetails
                .First(d => d.Id == exportInvoiceDetail1.Id);

            Assert.That(modifiedDetail.Quantity, Is.EqualTo(inputModel.ExportedProducts[0].Quantity));
            Assert.That(modifiedDetail.UnitPrice, Is.EqualTo(inputModel.ExportedProducts[0].UnitPrice));

            // 2. Assert unchanged detail
            var untouchedDetail = capturedExportInvoice.ExportInvoicesDetails
                .First(d => d.Id == exportInvoiceDetail2.Id);

            Assert.That(untouchedDetail.Quantity, Is.EqualTo(exportInvoiceDetail2.Quantity));
            Assert.That(untouchedDetail.UnitPrice, Is.EqualTo(exportInvoiceDetail2.UnitPrice));

            // 3. Assert new dynamically added detail
            var actual = capturedExportInvoice.ExportInvoicesDetails.ElementAt(2);

            Assert.That(actual.Quantity, Is.EqualTo(inputModel.ExportedProducts[2].Quantity));
            Assert.That(actual.UnitPrice, Is.EqualTo(inputModel.ExportedProducts[2].UnitPrice));
        }

        [Test]
        public async Task ShouldUpdateImportInvoiceSuccessfully_WhenRemovingExistingItem()
        {
            // Arrange
            inputModel.ExportedProducts.RemoveAt(1);

            ExportInvoice? capturedExportInvoice = null;

            exportInvoiceRepo
                .Setup(r => r.SaveChangesAsync())
                .Callback(() => capturedExportInvoice = exportInvoice)
                .Returns(Task.CompletedTask);

            List<ExportInvoiceDetail>? detailsToRemove = null;

            exportInvoiceDetailRepo
                .Setup(r => r.DeleteRange(It.IsAny<IEnumerable<ExportInvoiceDetail>>()))
                .Callback<IEnumerable<ExportInvoiceDetail>>(details =>
                {
                    detailsToRemove = details.ToList();
                });

            // Act
            var result = await exportInvoiceService.UpdateExportInvoiceAsync(inputModel, userId);

            foreach (var detail in detailsToRemove!)
            {
                capturedExportInvoice!.ExportInvoicesDetails.Remove(detail);
            }

            // Assert
            Assert.That(result.Success, Is.True);

            Assert.That(capturedExportInvoice, Is.Not.Null);

            Assert.That(capturedExportInvoice!.ExportInvoicesDetails.Count, Is.EqualTo(1));

            var remainingDetail = capturedExportInvoice.ExportInvoicesDetails.First();

            var expectedDetail = inputModel.ExportedProducts[0];

            Assert.That(remainingDetail.Quantity, Is.EqualTo(expectedDetail.Quantity));
            Assert.That(remainingDetail.UnitPrice, Is.EqualTo(expectedDetail.UnitPrice));
            Assert.That(remainingDetail.Id, Is.EqualTo(expectedDetail.Id));
        }
    }
}
