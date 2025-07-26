using Moq;

using WarehouseApp.Data.Models;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

namespace WarehouseApp.Services.Tests.WarehouseServiceTests
{
    [TestFixture]
    public class GetWarehouseForEditingAsyncTests : WarehouseServiceBaseTests
    {
        [Test]
        public async Task ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            SetupUserNotFound();

            // Act
            var result = await warehouseService.GetWarehouseForEditingAsync(Guid.NewGuid(), userId);

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
            warehouseRepo.Setup(x => x.GetByIdAsNoTrackingAsync(warehouseId))
                .ReturnsAsync((Warehouse?)null);

            // Act
            var result = await warehouseService.GetWarehouseForEditingAsync(warehouseId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(WarehouseNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenUserHasNoPermission()
        {
            var warehouseId = Guid.NewGuid();
            var warehouse = new Warehouse
            {
                Id = warehouseId,
                CreatedByUserId = Guid.NewGuid(), // Different user
                Name = "Test Warehouse",
                Address = "Some Address",
                Size = 50
            };

            // Arrange
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(validUser);
            warehouseRepo.Setup(x => x.GetByIdAsNoTrackingAsync(warehouseId))
                .ReturnsAsync(warehouse);

            // Act
            var result = await warehouseService.GetWarehouseForEditingAsync(warehouseId, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermission));
        }

        [Test]
        public async Task ShouldReturnCorrectModel_WhenUserIsOwner()
        {
            var warehouseId = Guid.NewGuid();
            var warehouse = new Warehouse
            {
                Id = warehouseId,
                CreatedByUserId = userId,
                Name = "Test Warehouse",
                Address = "Some Address",
                Size = 75
            };

            // Arrange
            userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(validUser);
            warehouseRepo.Setup(x => x.GetByIdAsNoTrackingAsync(warehouseId))
                .ReturnsAsync(warehouse);

            // Act
            var result = await warehouseService.GetWarehouseForEditingAsync(warehouseId, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            var model = result.Data!;
            Assert.That(model.Id, Is.EqualTo(warehouse.Id));
            Assert.That(model.Name, Is.EqualTo(warehouse.Name));
            Assert.That(model.Address, Is.EqualTo(warehouse.Address));
            Assert.That(model.Size, Is.EqualTo(warehouse.Size));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExceptionThrown()
        {
            // Arrange
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("DB Error"));

            // Act
            var result = await warehouseService.GetWarehouseForEditingAsync(Guid.NewGuid(), userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(GetModelFailure));
        }
    }
}