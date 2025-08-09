using Moq;
using MockQueryable;
using Microsoft.Extensions.Logging;

using WarehouseApp.Data.Models;
using WarehouseApp.Data.Models.Enums;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.AdminRequest;

namespace WarehouseApp.Services.Tests.UserServiceTests
{
    [TestFixture]
    public class CheckForExistingRequestAsyncTests : UserServiceBaseTests
    {

        private AdminRequest pendingRequest = new AdminRequest
        {
            UserId = userId,
            Status = AdminRequestStatus.Pending
        };
        private AdminRequest rejectedRequest = new AdminRequest
        {
            UserId = userId,
            Status = AdminRequestStatus.Rejected
        };
        private AdminRequest approvedRequest = new AdminRequest
        {
            UserId = userId,
            Status = AdminRequestStatus.Approved
        };

        [Test]
        public async Task ReturnsOk_WhenNoRequestExists()
        {
            // Arrange
            adminRequestRepo.Setup(r => r.All())
                .Returns(new List<AdminRequest>().AsQueryable().BuildMock());

            // Act
            var result = await userService.CheckForExistingRequestAsync(userId);

            // Assert
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task ReturnsFailureWithPendingRequest_WhenPendingRequestExists()
        {
            // Arrange
            adminRequestRepo.Setup(r => r.All())
                .Returns(new List<AdminRequest> { pendingRequest }.AsQueryable().BuildMock());

            userManager.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(validUser);

            // Act
            var result = await userService.CheckForExistingRequestAsync(userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(PendingRequest));
        }

        [Test]
        public async Task ReturnsFailureWithRejectedRequest_WhenRejectedRequestExists()
        {
            // Arrange
            adminRequestRepo.Setup(r => r.All())
                .Returns(new List<AdminRequest> { rejectedRequest }.AsQueryable().BuildMock());

            userManager.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(validUser);

            // Act
            var result = await userService.CheckForExistingRequestAsync(userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(RejectedRequest));
        }

        [Test]
        public async Task ReturnsFailureWithRevoked_WhenApprovedRequestExistsAndUserIsNotInAdminRole()
        {
            // Arrange
            adminRequestRepo.Setup(r => r.All())
                .Returns(new List<AdminRequest> { approvedRequest }.AsQueryable().BuildMock());

            userManager.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(validUser);

            userManager.Setup(u => u.IsInRoleAsync(validUser, AdministratorRole))
                .ReturnsAsync(false);

            // Act
            var result = await userService.CheckForExistingRequestAsync(userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(Revoked));
        }

        [Test]
        public async Task ReturnsFailureWithAlreadyApproved_WhenApprovedRequestExistsAndUserIsInAdminRole()
        {
            // Arrange
            adminRequestRepo.Setup(r => r.All())
                .Returns(new List<AdminRequest> { approvedRequest }.AsQueryable().BuildMock());

            userManager.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(validUser);

            userManager.Setup(u => u.IsInRoleAsync(validUser, AdministratorRole))
                .ReturnsAsync(true);

            // Act
            var result = await userService.CheckForExistingRequestAsync(userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(AlreadyApproved));
        }

        [Test]
        public async Task ReturnsFailureAndLogsError_WhenExceptionIsThrown()
        {
            // Arrange
            adminRequestRepo.Setup(r => r.All())
                .Throws(new Exception("DB failure"));

            // Act
            var result = await userService.CheckForExistingRequestAsync(userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(SubmittingFailure));

            logger.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(SubmittingFailure)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }
    }
}
