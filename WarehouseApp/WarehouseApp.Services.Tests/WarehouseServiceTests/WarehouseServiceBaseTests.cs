using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Services.Data;

namespace WarehouseApp.Services.Tests.WarehouseServiceTests
{
    [TestFixture]
    public abstract class WarehouseServiceBaseTests
    {
        protected Mock<UserManager<ApplicationUser>> userManager;
        protected Mock<IWarehouseRepository> warehouseRepo;
        protected Mock<IApplicationUserWarehouseRepository> appUserWarehouseRepo;
        protected Mock<IImportInvoiceDetailRepository> importInvoiceDetailRepo;
        protected Mock<ILogger<WarehouseService>> logger;
        protected WarehouseService warehouseService;

        protected static readonly Guid userId = Guid.Parse("C994999B-02DD-46C2-ABC4-00C4787E629F");

        protected ApplicationUser validUser;
        protected List<Warehouse> warehousesList;

        [SetUp]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userManager = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            validUser = new ApplicationUser { Id = userId };

            userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
               .ReturnsAsync(validUser);

            warehouseRepo = new Mock<IWarehouseRepository>();
            appUserWarehouseRepo = new Mock<IApplicationUserWarehouseRepository>();
            importInvoiceDetailRepo = new Mock<IImportInvoiceDetailRepository>();

            logger = new Mock<ILogger<WarehouseService>>();

            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));

            warehouseService = new WarehouseService(
                userManager.Object,
                warehouseRepo.Object,
                appUserWarehouseRepo.Object,
                importInvoiceDetailRepo.Object,
                logger.Object);

            warehousesList = new List<Warehouse>
            {
                new Warehouse
                {
                    Id = Guid.Parse("A1F7B60E-9C39-4E28-B2BD-35E750C6FBAE"),
                    Name = "Alpha Warehouse",
                    Address = "123 Alpha St",
                    CreatedDate = new DateTime(2022, 5, 1),
                    Size = 100,
                    WarehouseUsers = new List<ApplicationUserWarehouse> {
                        new ApplicationUserWarehouse { ApplicationUserId = userId }
                    }
                },
                new Warehouse
                {
                    Id = Guid.Parse("B8C13E6A-798C-4D49-8A34-1C399F77C37A"),
                    Name = "Beta Warehouse",
                    Address = "456 Beta Rd",
                    CreatedDate = new DateTime(2023, 7, 15),
                    Size = 200,
                    WarehouseUsers = new List<ApplicationUserWarehouse> {
                        new ApplicationUserWarehouse { ApplicationUserId = userId }
                    }
                }
            };
        }

        protected void SetupUserNotFound() =>
           userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync((ApplicationUser?)null);
    }
}
