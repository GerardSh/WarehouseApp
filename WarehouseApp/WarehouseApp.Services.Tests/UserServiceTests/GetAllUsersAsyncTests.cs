using MockQueryable;
using Moq;

using WarehouseApp.Data.Models;
using WarehouseApp.Web.ViewModels.Admin.UserManagement;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.UserManager;

namespace WarehouseApp.Services.Tests.UserServiceTests
{
    [TestFixture]
    public class GetAllUsersAsyncTests : UserServiceBaseTests
    {
        [Test]
        public async Task ReturnsAllUsers()
        {
            // Arrange
            userManager.Setup(x => x.Users)
                .Returns(usersList.AsQueryable().BuildMock());

            roleManager.Setup(r => r.Roles)
                .Returns(rolesList.AsQueryable().BuildMock());

            userManager.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "User" });

            var input = new AllUsersWithRolesSearchFilterViewModel();

            // Act
            var result = await userService.GetAllUsersAsync(input, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(input.Users.Count(), Is.EqualTo(2));
            Assert.That(input.TotalItemsBeforePagination, Is.EqualTo(2));
            Assert.That(input.TotalUsers, Is.EqualTo(2));
            Assert.That(input.TotalPages, Is.EqualTo(1));

            var firstUser = input.Users.FirstOrDefault(u => u.Email == "a@example.com");
            var secondUser = input.Users.FirstOrDefault(u => u.Email == "b@example.com");

            Assert.That(firstUser, Is.Not.Null);
            Assert.That(firstUser!.Roles, Does.Contain("User"));

            Assert.That(secondUser, Is.Not.Null);
            Assert.That(secondUser!.Roles, Does.Contain("User"));
        }

        [Test]
        public async Task ReturnFailure_WhenUserIsNotFound()
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
        public async Task FiltersByEmail_WhenSearchQueryIsProvided()
        {
            // Arrange
            var inputModel = new AllUsersWithRolesSearchFilterViewModel
            {
                SearchQuery = "b@example.com",
                EntitiesPerPage = 5,
                CurrentPage = 1
            };

            userManager.Setup(x => x.Users).Returns(usersList.AsQueryable().BuildMock());

            roleManager.Setup(r => r.Roles)
                .Returns(rolesList.AsQueryable().BuildMock());

            userManager.Setup(u => u.GetRolesAsync(usersList[1]))
                .ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await userService.GetAllUsersAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Users, Has.Exactly(1).Items);
            Assert.That(inputModel.TotalUsers, Is.EqualTo(2));
            Assert.That(inputModel.TotalItemsBeforePagination, Is.EqualTo(1));

            var user = inputModel.Users.First();
            Assert.Multiple(() =>
            {
                Assert.That(user.Email, Is.EqualTo("b@example.com"));
                Assert.That(user.Id, Is.EqualTo(usersList[1].Id.ToString()));
                Assert.That(user.Roles, Contains.Item("User"));
            });
        }

        [Test]
        public async Task FiltersByEmail_CaseInsensitive()
        {
            // Arrange
            var inputModel = new AllUsersWithRolesSearchFilterViewModel
            {
                SearchQuery = "B@EXAMPLE.com"
            };

            userManager.Setup(x => x.Users).Returns(usersList.AsQueryable().BuildMock());

            roleManager.Setup(r => r.Roles)
                .Returns(rolesList.AsQueryable().BuildMock());

            userManager.Setup(u => u.GetRolesAsync(usersList[1]))
                .ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await userService.GetAllUsersAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(inputModel.Users.Count(), Is.EqualTo(1));
            Assert.That(inputModel.Users.First().Email, Is.EqualTo("b@example.com"));
        }

        [Test]
        public async Task PopulatesAllRolesCorrectly()
        {
            // Arrange
            userManager.Setup(x => x.Users).Returns(usersList.AsQueryable().BuildMock());
            roleManager.Setup(r => r.Roles)
                .Returns(rolesList.AsQueryable().BuildMock());

            userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(validUser);

            userManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "User" });

            var input = new AllUsersWithRolesSearchFilterViewModel();

            // Act
            var result = await userService.GetAllUsersAsync(input, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(input.AllRoles, Is.EquivalentTo(rolesList.Select(r => r.Name)));
        }

        [Test]
        public async Task RespectsPagination()
        {
            // Arrange
            userManager.Setup(x => x.Users)
                .Returns(usersList.AsQueryable().BuildMock());

            roleManager.Setup(r => r.Roles)
                .Returns(rolesList.AsQueryable().BuildMock());

            userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(validUser);

            userManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "User" });

            var input = new AllUsersWithRolesSearchFilterViewModel
            {
                EntitiesPerPage = 1,
                CurrentPage = 2
            };

            // Act
            var result = await userService.GetAllUsersAsync(input, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(input.Users.Count(), Is.EqualTo(1));
            Assert.That(input.Users.First().Email, Is.EqualTo("b@example.com"));
            Assert.That(input.TotalItemsBeforePagination, Is.EqualTo(2));
            Assert.That(input.TotalPages, Is.EqualTo(2));
        }

        [TestCase(0, -1, 5, 1)]
        [TestCase(100, 1, 100, 1)]
        [TestCase(10, 1000, 10, 1)]
        [TestCase(10, 0, 10, 1)]
        [TestCase(5, 1, 5, 1)]
        public async Task PaginationValidation(
            int entitiesPerPageInput,
            int currentPageInput,
            int expectedEntitiesPerPage,
            int expectedCurrentPage)
        {
            // Arrange
            userManager.Setup(x => x.Users)
                .Returns(usersList.AsQueryable().BuildMock());

            userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(validUser);

            roleManager.Setup(r => r.Roles)
                .Returns(rolesList.AsQueryable().BuildMock());

            userManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "User" });

            var input = new AllUsersWithRolesSearchFilterViewModel
            {
                EntitiesPerPage = entitiesPerPageInput,
                CurrentPage = currentPageInput
            };

            // Act
            var result = await userService.GetAllUsersAsync(input, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(input.EntitiesPerPage, Is.EqualTo(expectedEntitiesPerPage));
            Assert.That(input.CurrentPage, Is.EqualTo(expectedCurrentPage));
        }

        [Test]
        public async Task ReturnsFailureOnException()
        {
            // Arrange
            userManager.Setup(x => x.Users).Throws<Exception>();

            var input = new AllUsersWithRolesSearchFilterViewModel();

            // Act
            var result = await userService.GetAllUsersAsync(input, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(GetAllUsersFailure));
        }
    }
}
