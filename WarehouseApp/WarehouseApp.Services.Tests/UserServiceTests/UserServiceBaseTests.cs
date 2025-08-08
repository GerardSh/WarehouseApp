using Moq;
using Microsoft.AspNetCore.Identity;

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
        protected Mock<IAdminRequestRepository> adminRequestRepo;
        protected UserService userService;

        protected static readonly string UserRole = "User";
        protected static readonly string AdministratorRole = "Administrator";
        protected static readonly Guid userId = Guid.Parse("11F7B60E-9C39-4E28-B2BD-35E750C6FBAE");

        protected ApplicationUser validUser;
        protected List<ApplicationUser> usersList;
        protected List<IdentityRole<Guid>> rolesList;

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

            adminRequestRepo = new Mock<IAdminRequestRepository>();

            userService = new UserService(
                userManager.Object,
                roleManager.Object,
                appUserWarehouseRepo.Object,
                adminRequestRepo.Object,
                warehouseService.Object);

            usersList = new List<ApplicationUser>
            {
                new ApplicationUser { Id = Guid.Parse("11F7B60E-9C39-4E28-B2BD-35E750C6FBAE"), Email = "a@example.com" },
                new ApplicationUser { Id = Guid.Parse("22C13E6A-798C-4D49-8A34-1C399F77C37A"), Email = "b@example.com" }
            };

            rolesList = new List<IdentityRole<Guid>>
            {
                new IdentityRole<Guid>("Administrator"),
                new IdentityRole<Guid>("User")
            };
        }

        protected void SetupUserNotFound() =>
           userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync((ApplicationUser?)null);
    }
}
