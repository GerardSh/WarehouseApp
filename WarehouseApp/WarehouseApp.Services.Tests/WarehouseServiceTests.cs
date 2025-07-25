using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;

using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository.Interfaces;
using WarehouseApp.Services.Data;
using WarehouseApp.Web.ViewModels.Shared;
using WarehouseApp.Web.ViewModels.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

namespace WarehouseApp.Services.Tests
{
    [TestFixture]
    public class WarehouseServiceTests
    {
        private Mock<UserManager<ApplicationUser>> userManager;
        private Mock<IWarehouseRepository> warehouseRepo;
        private Mock<IApplicationUserWarehouseRepository> appUserWarehouseRepo;
        private Mock<IImportInvoiceDetailRepository> importInvoiceDetailRepo;
        private WarehouseService warehouseService;

        private static readonly Guid userId = Guid.Parse("C994999B-02DD-46C2-ABC4-00C4787E629F");

        private ApplicationUser validUser;
        private List<Warehouse> warehousesList;

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

            warehouseService = new WarehouseService(
                userManager.Object,
                warehouseRepo.Object,
                appUserWarehouseRepo.Object,
                importInvoiceDetailRepo.Object);

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

        [Test]
        public async Task GetWarehousesForUserAsync_ReturnsAllWarehouses()
        {
            // Arrange
            warehouseRepo.Setup(r => r.All())
                .Returns(warehousesList.AsQueryable().BuildMock());

            var input = new AllWarehousesSearchFilterViewModel();

            // Act
            var result = await warehouseService.GetWarehousesForUserAsync(input, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(input.TotalUserWarehouses, Is.EqualTo(2));
            Assert.That(input.TotalItemsBeforePagination, Is.EqualTo(2));
            Assert.That(input.Warehouses.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetWarehousesForUserAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                           .ReturnsAsync((ApplicationUser?)null);

            var input = new AllWarehousesSearchFilterViewModel();

            // Act
            var result = await warehouseService.GetWarehousesForUserAsync(input, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task GetWarehousesForUserAsync_FiltersBySearchQuery()
        {
            // Arrange
            warehouseRepo.Setup(x => x.All())
                             .Returns(warehousesList.AsQueryable().BuildMock());

            var input = new AllWarehousesSearchFilterViewModel
            {
                SearchQuery = "Beta",
                EntitiesPerPage = 10,
                CurrentPage = 1
            };

            // Act
            var result = await warehouseService.GetWarehousesForUserAsync(input, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(input.TotalUserWarehouses, Is.EqualTo(2));
            Assert.That(input.TotalItemsBeforePagination, Is.EqualTo(1));
            Assert.That(input.Warehouses.Count(), Is.EqualTo(1));
            Assert.That(input.Warehouses.ElementAt(0).Name, Is.EqualTo("Beta Warehouse"));
        }

        [Test]
        public async Task GetWarehousesForUserAsync_FiltersBySingleYear()
        {
            // Arrange
            warehouseRepo.Setup(x => x.All())
                             .Returns(warehousesList.AsQueryable().BuildMock());

            var input = new AllWarehousesSearchFilterViewModel
            {
                YearFilter = "2022",
                EntitiesPerPage = 10,
                CurrentPage = 1
            };

            // Act
            var result = await warehouseService.GetWarehousesForUserAsync(input, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(input.Warehouses.Count(), Is.EqualTo(1));
            Assert.That(input.Warehouses.ElementAt(0).Name, Is.EqualTo("Alpha Warehouse"));
        }

        [Test]
        public async Task GetWarehousesForUserAsync_FiltersByYearRange()
        {
            // Arrange
            warehouseRepo.Setup(x => x.All())
                             .Returns(warehousesList.AsQueryable().BuildMock());

            var input = new AllWarehousesSearchFilterViewModel
            {
                YearFilter = "2021-2022",
                EntitiesPerPage = 10,
                CurrentPage = 1
            };

            // Act
            var result = await warehouseService.GetWarehousesForUserAsync(input, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(input.Warehouses.Count(), Is.EqualTo(1));
            Assert.That(input.Warehouses.ElementAt(0).Name, Is.EqualTo("Alpha Warehouse"));
        }

        [Test]
        public async Task GetWarehousesForUserAsync_RespectsPagination()
        {
            // Arrange
            warehouseRepo.Setup(x => x.All())
                             .Returns(warehousesList.AsQueryable().BuildMock());

            var input = new AllWarehousesSearchFilterViewModel
            {
                EntitiesPerPage = 1,
                CurrentPage = 2
            };

            // Act
            var result = await warehouseService.GetWarehousesForUserAsync(input, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(input.Warehouses.Count(), Is.EqualTo(1));
            Assert.That(input.Warehouses.ElementAt(0).Name, Is.EqualTo("Beta Warehouse"));
            Assert.That(input.TotalItemsBeforePagination, Is.EqualTo(2));
            Assert.That(input.TotalPages, Is.EqualTo(2));
        }

        [TestCase(0, -1, 5, 1)]
        [TestCase(150, 1, 100, 1)]  
        [TestCase(10, 1000, 10, 1)] 
        [TestCase(10, 0, 10, 1)]    
        [TestCase(5, 1, 5, 1)]    
        public async Task GetWarehousesForUserAsync_PaginationValidation(
            int entitiesPerPageInput, int currentPageInput, int expectedEntitiesPerPage, int expectedCurrentPage)
        {
            // Arrange
            warehouseRepo.Setup(x => x.All())
                         .Returns(warehousesList.AsQueryable().BuildMock());

            var input = new AllWarehousesSearchFilterViewModel
            {
                EntitiesPerPage = entitiesPerPageInput,
                CurrentPage = currentPageInput
            };

            // Act
            var result = await warehouseService.GetWarehousesForUserAsync(input, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(input.EntitiesPerPage, Is.EqualTo(expectedEntitiesPerPage));
            Assert.That(input.CurrentPage, Is.EqualTo(expectedCurrentPage));
        }

        [Test]
        public async Task GetWarehousesForUserAsync_ReturnsFailureOnException()
        {
            // Arrange
            userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                           .ThrowsAsync(new Exception("Simulated failure"));

            var input = new AllWarehousesSearchFilterViewModel();

            // Act
            var result = await warehouseService.GetWarehousesForUserAsync(input, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(RetrievingFailure));
        }

        [Test]
        public async Task CreateWarehouseAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);

            var inputModel = new CreateWarehouseInputModel
            {
                Name = "New Warehouse",
                Address = "123 New St",
                Size = 50
            };

            // Act
            var result = await warehouseService.CreateWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task CreateWarehouseAsync_ShouldReturnFailure_WhenWarehouseNameAlreadyExists()
        {
            // Arrange
            var inputModel = new CreateWarehouseInputModel
            {
                Name = "Alpha Warehouse",
                Address = "123 New St",
                Size = 50
            };

            appUserWarehouseRepo.Setup(x => x.UserHasWarehouseWithNameAsync(userId, inputModel.Name))
                .ReturnsAsync(true);

            // Act
            var result = await warehouseService.CreateWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(WarehouseDuplicateName));
        }

        [Test]
        public async Task CreateWarehouseAsync_ShouldReturnSuccess_WhenWarehouseCreated()
        {
            // Arrange
            var inputModel = new CreateWarehouseInputModel
            {
                Name = "Gamma Warehouse",
                Address = "789 Gamma Ave",
                Size = 150
            };

            appUserWarehouseRepo.Setup(x => x.UserHasWarehouseWithNameAsync(userId, inputModel.Name))
                .ReturnsAsync(false);

            appUserWarehouseRepo.Setup(x => x.AddAsync(It.IsAny<ApplicationUserWarehouse>()))
                .Returns(Task.CompletedTask);

            appUserWarehouseRepo.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await warehouseService.CreateWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);

            appUserWarehouseRepo.Verify(x => x.AddAsync(It.Is<ApplicationUserWarehouse>(u =>
                u.ApplicationUser == validUser &&
                u.Warehouse.Name == inputModel.Name &&
                u.Warehouse.Address == inputModel.Address &&
                u.Warehouse.Size == inputModel.Size
            )), Times.Once);

            appUserWarehouseRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task CreateWarehouseAsync_ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            var inputModel = new CreateWarehouseInputModel
            {
                Name = "Delta Warehouse",
                Address = "456 Delta Rd",
                Size = 75
            };

            appUserWarehouseRepo.Setup(x => x.UserHasWarehouseWithNameAsync(userId, inputModel.Name))
                .ReturnsAsync(false);

            appUserWarehouseRepo.Setup(x => x.AddAsync(It.IsAny<ApplicationUserWarehouse>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await warehouseService.CreateWarehouseAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(CreationFailure));
        }
    }
}
