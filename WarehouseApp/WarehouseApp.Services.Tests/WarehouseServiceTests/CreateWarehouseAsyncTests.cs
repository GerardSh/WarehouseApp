using Microsoft.Extensions.Logging;
using Moq;

using WarehouseApp.Data.Models;
using WarehouseApp.Web.ViewModels.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

namespace WarehouseApp.Services.Tests.WarehouseServiceTests
{
    [TestFixture]
    public class CreateWarehouseAsyncTests : WarehouseServiceBaseTests
    {

        [Test]
        public async Task ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            SetupUserNotFound();

            var inputModel = new CreateWarehouseInputModel();

            // Act
            var result = await warehouseService.CreateWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenWarehouseNameAlreadyExists()
        {
            // Arrange
            var inputModel = new CreateWarehouseInputModel
            {
                Name = "Alpha Warehouse",
                Address = "123 New St",
                Size = 50
            };

            appUserWarehouseRepo.Setup(x => x.UserHasWarehouseWithNameAsync(userId, inputModel.Name))
                .ReturnsAsync(true);

            // Act
            var result = await warehouseService.CreateWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(WarehouseDuplicateName));
        }

        [Test]
        public async Task ShouldReturnSuccess_WhenWarehouseCreated()
        {
            // Arrange
            var inputModel = new CreateWarehouseInputModel
            {
                Name = "Gamma Warehouse",
                Address = "789 Gamma Ave",
                Size = 150
            };

            appUserWarehouseRepo.Setup(x => x.UserHasWarehouseWithNameAsync(userId, inputModel.Name))
                .ReturnsAsync(false);

            appUserWarehouseRepo.Setup(x => x.AddAsync(It.IsAny<ApplicationUserWarehouse>()))
                .Returns(Task.CompletedTask);

            appUserWarehouseRepo.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await warehouseService.CreateWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);

            appUserWarehouseRepo.Verify(x => x.AddAsync(It.Is<ApplicationUserWarehouse>(u =>
                u.ApplicationUser == validUser &&
                u.Warehouse.Name == inputModel.Name &&
                u.Warehouse.Address == inputModel.Address &&
                u.Warehouse.Size == inputModel.Size
            )), Times.Once);

            appUserWarehouseRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            var inputModel = new CreateWarehouseInputModel
            {
                Name = "Delta Warehouse",
                Address = "456 Delta Rd",
                Size = 75
            };

            appUserWarehouseRepo.Setup(x => x.UserHasWarehouseWithNameAsync(userId, inputModel.Name))
            .ReturnsAsync(false);

            appUserWarehouseRepo.Setup(x => x.AddAsync(It.IsAny<ApplicationUserWarehouse>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await warehouseService.CreateWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(CreationFailure));

            logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(CreationFailure)),
                It.Is<Exception>(ex => ex.Message == "Database error"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
