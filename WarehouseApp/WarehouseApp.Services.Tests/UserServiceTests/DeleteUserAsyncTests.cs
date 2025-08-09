using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.UserManager;

namespace WarehouseApp.Services.Tests.UserServiceTests
{
    [TestFixture]
    public class DeleteUserAsyncTests : UserServiceBaseTests
    {
        private readonly Guid warehouseId = Guid.NewGuid();

        [Test]
        public async Task ReturnsFailure_WhenUserDoesNotExist()
        {
            // Arrange
            SetupUserNotFound();

            // Act
            var result = await userService.DeleteUserAsync(userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task ReturnsSuccess_WhenUserExistsAndHasNoMappings()
        {
            // Arrange
            appUserWarehouseRepo.Setup(r => r.GetAllByUserIdAsync(userId)).ReturnsAsync(new List<ApplicationUserWarehouse>());
            userManager.Setup(um => um.DeleteAsync(validUser)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await userService.DeleteUserAsync(userId);

            // Assert
            Assert.That(result.Success, Is.True);
            appUserWarehouseRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ReturnsSuccess_WhenUserHasMappingsButWarehousesHaveOtherUsers()
        {
            // Arrange
            var mappings = new List<ApplicationUserWarehouse>
            {
                new ApplicationUserWarehouse { WarehouseId = warehouseId, ApplicationUserId = userId }
            };

            appUserWarehouseRepo.Setup(r => r.GetAllByUserIdAsync(userId)).ReturnsAsync(mappings);
            appUserWarehouseRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<ApplicationUserWarehouse, bool>>>()))
                                .ReturnsAsync(true);

            userManager.Setup(um => um.DeleteAsync(validUser)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await userService.DeleteUserAsync(userId);

            // Assert
            Assert.That(result.Success, Is.True);
            warehouseService.Verify(ws => ws.MarkAsDeletedWithoutSavingAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task ReturnsSuccess_WhenUserHasMappingsAndWarehouseHasNoOtherUsers()
        {
            // Arrange
            var mappings = new List<ApplicationUserWarehouse>
            {
                new ApplicationUserWarehouse { WarehouseId = warehouseId, ApplicationUserId = userId }
            };

            appUserWarehouseRepo.Setup(r => r.GetAllByUserIdAsync(userId)).ReturnsAsync(mappings);
            appUserWarehouseRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<ApplicationUserWarehouse, bool>>>()))
                                .ReturnsAsync(false);

            warehouseService.Setup(ws => ws.MarkAsDeletedWithoutSavingAsync(warehouseId))
                            .ReturnsAsync(OperationResult.Ok());

            userManager.Setup(um => um.DeleteAsync(validUser)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await userService.DeleteUserAsync(userId);

            // Assert
            Assert.That(result.Success, Is.True);
            warehouseService.Verify(ws => ws.MarkAsDeletedWithoutSavingAsync(warehouseId), Times.Once);
        }

        [Test]
        public async Task Succeeds_WhenWarehouseAlreadyDeleted()
        {
            // Arrange
            var mappings = new List<ApplicationUserWarehouse>
            {
                new ApplicationUserWarehouse { WarehouseId = warehouseId, ApplicationUserId = userId }
            };

            appUserWarehouseRepo.Setup(r => r.GetAllByUserIdAsync(userId)).ReturnsAsync(mappings);
            appUserWarehouseRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<ApplicationUserWarehouse, bool>>>()))
                                .ReturnsAsync(false);

            warehouseService.Setup(ws => ws.MarkAsDeletedWithoutSavingAsync(warehouseId))
                            .ReturnsAsync(OperationResult.Failure(FailedToMarkWarehouse));

            userManager.Setup(um => um.FindByIdAsync(userId.ToString()))
                       .ReturnsAsync(new ApplicationUser { Id = userId });

            userManager.Setup(um => um.DeleteAsync(It.IsAny<ApplicationUser>()))
                       .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await userService.DeleteUserAsync(userId);

            // Assert
            Assert.That(result.Success, Is.True);
            warehouseService.Verify(ws => ws.MarkAsDeletedWithoutSavingAsync(warehouseId), Times.Once);
        }

        [Test]
        public async Task ReturnsFailure_WhenUserDeletionFails()
        {
            // Arrange
            appUserWarehouseRepo.Setup(r => r.GetAllByUserIdAsync(userId)).ReturnsAsync(new List<ApplicationUserWarehouse>());
            userManager.Setup(um => um.DeleteAsync(validUser)).ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await userService.DeleteUserAsync(userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(FailedToDeleteUser));
        }

        [Test]
        public async Task ReturnsFailure_WhenSaveChangesThrowsException()
        {
            // Arrange
            appUserWarehouseRepo.Setup(r => r.GetAllByUserIdAsync(userId)).ReturnsAsync(new List<ApplicationUserWarehouse>());
            userManager.Setup(um => um.DeleteAsync(validUser)).ReturnsAsync(IdentityResult.Success);

            appUserWarehouseRepo.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await userService.DeleteUserAsync(userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(DeletionFailure));

            logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(DeletionFailure)),
                It.Is<Exception>(ex => ex.Message == "Database error"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
