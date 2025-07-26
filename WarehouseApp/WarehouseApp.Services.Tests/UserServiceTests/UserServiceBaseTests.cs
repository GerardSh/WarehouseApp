using Microsoft.AspNetCore.Identity;
using Moq;

using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Services.Data;
using WarehouseApp.Services.Data.Interfaces;

namespace WarehouseApp.Services.Tests.UserServiceTests
{
    [TestFixture]
    public abstract class UserServiceBaseTests
    {
        protected Mock<UserManager<ApplicationUser>> userManager;
        protected Mock<RoleManager<IdentityRole<Guid>>> roleManager;
        protected Mock<IWarehouseService> warehouseService;
        protected Mock<IApplicationUserWarehouseRepository> appUserWarehouseRepo;
        protected UserService userService;

        protected static readonly string UserRole = "User";
        protected static readonly string AdministratorRole = "Administrator";
        protected static readonly Guid userId = Guid.Parse("C994999B-02DD-46C2-ABC4-00C4787E629F");

        protected ApplicationUser validUser;

        [SetUp]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userManager = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            validUser = new ApplicationUser { Id = userId };

            userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
               .ReturnsAsync(validUser);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole<Guid>>>();
            roleManager = new Mock<RoleManager<IdentityRole<Guid>>>(
                roleStoreMock.Object, null, null, null, null);

            warehouseService = new Mock<IWarehouseService>();

            appUserWarehouseRepo = new Mock<IApplicationUserWarehouseRepository>();


            userService = new UserService(
                userManager.Object,
                roleManager.Object,
                warehouseService.Object,
                appUserWarehouseRepo.Object);
        }

        protected void SetupUserNotFound() =>
           userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync((ApplicationUser?)null);
    }
}
