using Moq;

using WarehouseApp.Data.Models;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;
using static WarehouseApp.Common.Constants.ApplicationConstants;

namespace WarehouseApp.Services.Tests.WarehouseServiceTests
{
    [TestFixture]
    public class GetWarehouseDetailsAsyncTests : WarehouseServiceBaseTests
    {
        [Test]
        public async Task ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            SetupUserNotFound();

            // Act
            var result = await warehouseService
                .GetWarehouseDetailsAsync(Guid.NewGuid(), userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenWarehouseNotFound()
        {
            var warehouseId = Guid.NewGuid();

            // Arrange
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(validUser);

            warehouseRepo.Setup(x => x.GetWarehouseDetailsByIdAsync(warehouseId))
                .ReturnsAsync((Warehouse?)null);

            // Act
            var result = await warehouseService.GetWarehouseDetailsAsync(warehouseId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(WarehouseNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenWarehouseIsDeleted()
        {
            // Arrange
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(validUser);

            var deletedWarehouse = warehousesList.ElementAt(0);
            deletedWarehouse.IsDeleted = true;

            warehouseRepo.Setup(x => x.GetWarehouseDetailsByIdAsync(deletedWarehouse.Id))
                .ReturnsAsync(deletedWarehouse);

            // Act
            var result = await warehouseService.GetWarehouseDetailsAsync(deletedWarehouse.Id, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(WarehouseNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenUserHasNoPermission()
        {
            // Arrange
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(validUser);
            var warehouse = warehousesList.ElementAt(0);

            warehouseRepo.Setup(x => x.GetWarehouseDetailsByIdAsync(warehouse.Id))
                .ReturnsAsync(warehouse);

            // Act
            var result = await warehouseService.GetWarehouseDetailsAsync(warehouse.Id, Guid.NewGuid());

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermission));
        }

        [Test]
        public async Task ShouldReturnSuccess_WhenUserHasPermissionViaWarehouseUsers()
        {
            var warehouseId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var warehouse = new Warehouse
            {
                Id = warehouseId,
                Name = "Warehouse A",
                Address = "Address A",
                CreatedByUserId = Guid.NewGuid(),
                CreatedByUser = new ApplicationUser { Email = "owner@example.com" },
                CreatedDate = DateTime.Now,
                Size = 50,
                IsDeleted = false,
                ImportInvoices = new List<ImportInvoice>(),
                ExportInvoices = new List<ExportInvoice>(),
                WarehouseUsers = new List<ApplicationUserWarehouse>
            {
                new ApplicationUserWarehouse { ApplicationUserId = userId }
            }
            };

            userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(new ApplicationUser { Id = userId, Email = "user@example.com" });

            warehouseRepo.Setup(x => x.GetWarehouseDetailsByIdAsync(warehouseId))
                .ReturnsAsync(warehouse);

            importInvoiceDetailRepo.Setup(x => x.GetAvailableProductsByWarehouseIdAsync(warehouseId))
                .ReturnsAsync(new List<ImportInvoiceDetail>());

            var result = await warehouseService.GetWarehouseDetailsAsync(warehouseId, userId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.IsUserOwner, Is.False);
        }

        [Test]
        public async Task ShouldReturnZero_TotalAvailableGoodsWhenNoProducts()
        {
            var warehouseId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var warehouse = new Warehouse
            {
                Id = warehouseId,
                Name = "Warehouse C",
                Address = "Address C",
                CreatedByUserId = userId,
                CreatedByUser = new ApplicationUser { Email = "owner@example.com" },
                CreatedDate = DateTime.Now,
                Size = 100,
                IsDeleted = false,
                ImportInvoices = new List<ImportInvoice>(),
                ExportInvoices = new List<ExportInvoice>(),
                WarehouseUsers = new List<ApplicationUserWarehouse>()
            };

            userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(new ApplicationUser { Id = userId, Email = "owner@example.com" });

            warehouseRepo.Setup(x => x.GetWarehouseDetailsByIdAsync(warehouseId))
                .ReturnsAsync(warehouse);

            importInvoiceDetailRepo.Setup(x => x.GetAvailableProductsByWarehouseIdAsync(warehouseId))
                .ReturnsAsync(new List<ImportInvoiceDetail>());

            var result = await warehouseService.GetWarehouseDetailsAsync(warehouseId, userId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.TotalAvailableGoods, Is.EqualTo(0));
        }

        [Test]
        public async Task ShouldReturnCorrectDetails_WhenUserHasAccessAndProductsGrouped()
        {
            // Arrange
            var warehouseId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category { Name = "Electronics" };
            var productA = new Product { Id = Guid.NewGuid(), Name = "Laptop", Category = category };
            var productB = new Product { Id = Guid.NewGuid(), Name = "Mouse", Category = category };

            var invoiceDetailList = new List<ImportInvoiceDetail>
            {
                new ImportInvoiceDetail
                {
                    ProductId = productA.Id,
                    Quantity = 10,
                    Product = productA,
                    ExportInvoicesPerProduct = new List<ExportInvoiceDetail> { new ExportInvoiceDetail { Quantity = 3 } }
                },
                new ImportInvoiceDetail
                {
                    ProductId = productA.Id,
                    Quantity = 5,
                    Product = productA,
                    ExportInvoicesPerProduct = new List<ExportInvoiceDetail> { new ExportInvoiceDetail { Quantity = 4 } }
                },
                new ImportInvoiceDetail
                {
                    ProductId = productB.Id,
                    Quantity = 4,
                    Product = productB,
                    ExportInvoicesPerProduct = new List<ExportInvoiceDetail> { new ExportInvoiceDetail { Quantity = 1 } }
                }
            };

            var warehouse = new Warehouse
            {
                Id = warehouseId,
                Name = "Test Warehouse",
                Address = "123 Street",
                CreatedByUserId = userId,
                CreatedByUser = new ApplicationUser { Email = "owner@example.com" },
                CreatedDate = new DateTime(2024, 1, 1),
                Size = 100,
                IsDeleted = false,
                ImportInvoices = new List<ImportInvoice> { new ImportInvoice() { InvoiceNumber = "Inv01" }, new ImportInvoice() { InvoiceNumber = "Inv02" } },
                ExportInvoices = new List<ExportInvoice> { new ExportInvoice() { InvoiceNumber = "Exp01" } },
                WarehouseUsers = new List<ApplicationUserWarehouse>()
            };

            userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(new ApplicationUser { Id = userId, Email = "owner@example.com" });

            warehouseRepo.Setup(x => x.GetWarehouseDetailsByIdAsync(warehouseId))
            .ReturnsAsync(warehouse);

            importInvoiceDetailRepo.Setup(x => x.GetAvailableProductsByWarehouseIdAsync(warehouseId))
                .ReturnsAsync(invoiceDetailList);

            // Act
            var result = await warehouseService.GetWarehouseDetailsAsync(warehouseId, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            var model = result.Data!;
            Assert.That(model.Id, Is.EqualTo(warehouseId.ToString()));
            Assert.That(model.TotalAvailableGoods, Is.EqualTo(2));
            Assert.That(model.TotalImportInvoices, Is.EqualTo(2));
            Assert.That(model.TotalExportInvoices, Is.EqualTo(1));
            Assert.That(model.Name, Is.EqualTo(warehouse.Name));
            Assert.That(model.Address, Is.EqualTo(warehouse.Address));
            Assert.That(model.Size, Is.EqualTo("100"));
            Assert.That(model.IsUserOwner, Is.True);
            Assert.That(model.CreatedByUser, Is.EqualTo("owner@example.com"));
            Assert.That(model.CreatedDate, Is.EqualTo(new DateTime(2024, 1, 1).ToString(DateFormat)));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExceptionThrown()
        {
            // Arrange
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("DB Error"));

            // Act
            var result = await warehouseService.GetWarehouseDetailsAsync(warehousesList.ElementAt(0).Id, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(GetModelFailure));
        }
    }
}