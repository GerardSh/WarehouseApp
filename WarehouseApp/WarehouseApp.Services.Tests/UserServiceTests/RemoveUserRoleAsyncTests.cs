using Microsoft.AspNetCore.Identity;
using Moq;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.UserManager;

namespace WarehouseApp.Services.Tests.UserServiceTests
{
    [TestFixture]
    public class RemoveUserRoleAsyncTests : UserServiceBaseTests
    {
        [Test]
        public async Task ReturnsFailure_WhenUserDoesNotExist()
        {
            // Arrange
            SetupUserNotFound();

            // Act
            var result = await userService.RemoveUserRoleAsync(Guid.NewGuid(), UserRole);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task ReturnsFailure_WhenRoleDoesNotExist()
        {
            // Arrange
            roleManager.Setup(r => r.RoleExistsAsync(UserRole)).ReturnsAsync(false);

            // Act
            var result = await userService.RemoveUserRoleAsync(userId, UserRole);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(RoleNotFound));
        }

        [Test]
        public async Task ReturnsFailure_WhenRemoveFromRoleFails()
        {
            // Arrange
            roleManager.Setup(r => r.RoleExistsAsync(UserRole)).ReturnsAsync(true);
            userManager.Setup(um => um.IsInRoleAsync(validUser, UserRole)).ReturnsAsync(true);
            userManager.Setup(um => um.RemoveFromRoleAsync(validUser, UserRole))
                       .ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await userService.RemoveUserRoleAsync(userId, UserRole);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(FailedToRemoveRole));
        }

        [Test]
        public async Task ReturnsSuccess_WhenUserInRoleAndRemovedSuccessfully()
        {
            // Arrange
            roleManager.Setup(r => r.RoleExistsAsync(UserRole)).ReturnsAsync(true);
            userManager.Setup(um => um.IsInRoleAsync(validUser, UserRole)).ReturnsAsync(true);
            userManager.Setup(um => um.RemoveFromRoleAsync(validUser, UserRole))
                       .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await userService.RemoveUserRoleAsync(userId, UserRole);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [Test]
        public async Task ReturnsSuccess_WhenUserNotInRole()
        {
            // Arrange
            roleManager.Setup(r => r.RoleExistsAsync(UserRole)).ReturnsAsync(true);
            userManager.Setup(um => um.IsInRoleAsync(validUser, UserRole)).ReturnsAsync(false);

            // Act
            var result = await userService.RemoveUserRoleAsync(userId, UserRole);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [Test]
        public async Task ReturnsFailure_WhenExceptionIsThrown()
        {
            // Arrange
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                       .ThrowsAsync(new Exception("Unexpected"));

            // Act
            var result = await userService.RemoveUserRoleAsync(Guid.NewGuid(), UserRole);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(RemoveRoleFailure));
        }
    }
}
