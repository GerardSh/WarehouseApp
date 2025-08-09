using Microsoft.Extensions.Logging;
using Moq;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.UserManager;

namespace WarehouseApp.Services.Tests.UserServiceTests
{
    [TestFixture]
    public class UserExistsByIdAsyncTests : UserServiceBaseTests
    {
        [Test]
        public async Task ReturnsSuccess_WhenUserExists()
        {
            // Act
            var result = await userService.UserExistsByIdAsync(userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
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
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
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
            Assert.That(result.ErrorMessage, Is.EqualTo(UserExistsFailure));

            logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(UserExistsFailure)),
                It.Is<Exception>(ex => ex.Message == "Something went wrong"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
