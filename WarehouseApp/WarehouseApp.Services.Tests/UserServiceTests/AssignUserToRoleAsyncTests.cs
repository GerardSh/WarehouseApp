using Microsoft.AspNetCore.Identity;
using Moq;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.UserManager;

namespace WarehouseApp.Services.Tests.UserServiceTests
{
    [TestFixture]
    public class AssignUserToRoleAsyncTests : UserServiceBaseTests
    {
        [Test]
        public async Task ReturnsFailure_WhenUserDoesNotExist()
        {
            SetupUserNotFound();

            var result = await userService.AssignUserToRoleAsync(Guid.NewGuid(), UserRole);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task ReturnsFailure_WhenRoleDoesNotExist()
        {
            // Arrange
            roleManager.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var result = await userService.AssignUserToRoleAsync(userId, "NonExistingRole");

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(RoleNotFound));
        }

        [Test]
        public async Task ReturnsSuccess_WhenUserAlreadyInRole()
        {
            // Arrange
            roleManager.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            userManager.Setup(x => x.IsInRoleAsync(validUser, UserRole))
                .ReturnsAsync(true);

            // Act
            var result = await userService.AssignUserToRoleAsync(userId, UserRole);

            // Assert
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task ReturnsFailure_WhenAddToRoleFails()
        {
            // Arrange
            roleManager.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            userManager.Setup(x => x.IsInRoleAsync(validUser, UserRole))
                .ReturnsAsync(false);

            userManager.Setup(x => x.AddToRoleAsync(validUser, UserRole))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed" }));

            // Act
            var result = await userService.AssignUserToRoleAsync(userId, UserRole);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(FailedToAssignRole));
        }

        [Test]
        public async Task ReturnsSuccess_WhenAddToRoleSucceeds()
        {
            // Arrange
            roleManager.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            userManager.Setup(x => x.IsInRoleAsync(validUser, UserRole))
                .ReturnsAsync(false);

            userManager.Setup(x => x.AddToRoleAsync(validUser, UserRole))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await userService.AssignUserToRoleAsync(userId, UserRole);

            // Assert
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task ReturnsFailure_WhenExceptionIsThrown()
        {
            // Arrange
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await userService.AssignUserToRoleAsync(userId, UserRole);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(AssignRoleFailure));
        }
    }
}
