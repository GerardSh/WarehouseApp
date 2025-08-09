using Moq;
using Microsoft.Extensions.Logging;

using WarehouseApp.Data.Models;
using WarehouseApp.Data.Models.Enums;
using WarehouseApp.Web.ViewModels.Admin.UserManagement;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.AdminRequest;

namespace WarehouseApp.Services.Tests.UserServiceTests
{
    [TestFixture]
    public class SubmitAdminRequestAsyncTests : UserServiceBaseTests
    {
        private AdminRequestFormModel validInputModel;

        [SetUp]
        public void TestSetup()
        {
            validInputModel = new AdminRequestFormModel { Reason = "Need admin rights" };
        }

        [Test]
        public async Task ReturnsSuccess_WhenRequestIsSubmittedSuccessfully()
        {
            // Arrange
            adminRequestRepo.Setup(r => r.AddAsync(It.IsAny<AdminRequest>())).Returns(Task.CompletedTask);
            adminRequestRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await userService.SubmitAdminRequestAsync(validInputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);

            adminRequestRepo.Verify(r => r.AddAsync(It.Is<AdminRequest>(
                ar => ar.UserId == userId
                && ar.Reason == validInputModel.Reason
                && ar.Status == AdminRequestStatus.Pending)), Times.Once);
            adminRequestRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ReturnsFailure_WhenSaveChangesAsyncThrowsException()
        {
            // Arrange
            adminRequestRepo.Setup(r => r.AddAsync(It.IsAny<AdminRequest>())).Returns(Task.CompletedTask);
            adminRequestRepo.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new Exception("Save error"));

            // Act
            var result = await userService.SubmitAdminRequestAsync(validInputModel, userId);

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
