using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using MockQueryable;
using Moq;

using WarehouseApp.Data.Models;
using WarehouseApp.Web.ViewModels.Admin.UserManagement;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.UserManager;

namespace WarehouseApp.Services.Tests.UserServiceTests
{
    [TestFixture]
    public class UserExistsByIdAsync_Tests : UserServiceBaseTests
    {
        [Test]
        public async Task ReturnsSuccess_WhenUserExists()
        {
            // Act
            var result = await userService.UserExistsByIdAsync(userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Empty);
        }

        [Test]
        public async Task ReturnsFailure_WhenUserDoesNotExist()
        {
            // Arrange
            SetupUserNotFound();

            // Act
            var result = await userService.UserExistsByIdAsync(Guid.NewGuid());

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Has.One.EqualTo(UserNotFound));
        }

        [Test]
        public async Task ReturnsFailure_WhenExceptionIsThrown()
        {
            // Arrange
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await userService.UserExistsByIdAsync(Guid.NewGuid());

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Has.One.EqualTo(UserExistsFailure));
        }
    }
}
