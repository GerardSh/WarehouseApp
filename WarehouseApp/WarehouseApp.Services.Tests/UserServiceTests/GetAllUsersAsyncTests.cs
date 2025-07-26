using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;
using WarehouseApp.Data.Models;
using WarehouseApp.Web.ViewModels.Admin.UserManagement;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;

namespace WarehouseApp.Services.Tests.UserServiceTests
{
    [TestFixture]
    public class GetAllUsersAsyncTests : UserServiceBaseTests
    {
        [Test]
        public async Task GetAllUsersAsync_ShouldReturnFailure_WhenUserIsNotFound()
        {
            // Arrange
            SetupUserNotFound();

            var inputModel = new AllUsersWithRolesSearchFilterViewModel();

            // Act
            var result = await userService.GetAllUsersAsync(inputModel, Guid.NewGuid());

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task GetAllUsersAsync_ShouldReturnSuccess_WhenValidUserAndNoSearch()
        {
            // Arrange
            var inputModel = new AllUsersWithRolesSearchFilterViewModel
            {
                EntitiesPerPage = 5,
                CurrentPage = 1
            };

            var usersList = new List<ApplicationUser>
            {
                new ApplicationUser { Id = Guid.NewGuid(), Email = "a@example.com" },
                new ApplicationUser { Id = Guid.NewGuid(), Email = "b@example.com" }
            }.AsQueryable().BuildMock();
         
                    userManager.Setup(x => x.Users).Returns(usersList);
         
                    roleManager.Setup(r => r.Roles).Returns(new List<IdentityRole<Guid>>
            {
                new IdentityRole<Guid>("Administrator"),
                new IdentityRole<Guid>("User")
            }.AsQueryable().BuildMock());

            userManager.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await userService.GetAllUsersAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Users, Is.Not.Null);
            Assert.That(inputModel.TotalUsers, Is.EqualTo(2));
            Assert.That(inputModel.TotalItemsBeforePagination, Is.EqualTo(2));
            Assert.That(inputModel.TotalPages, Is.EqualTo(1));
        }
    }
}
