using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;

using WarehouseApp.Web.ViewModels.Shared;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

namespace WarehouseApp.Services.Tests.WarehouseServiceTests
{
    [TestFixture]
    public class GetWarehousesForUserAsyncTests : WarehouseServiceBaseTests
    {

        [Test]
        public async Task ReturnsAllWarehouses()
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
        public async Task UserNotFound_ReturnsFailure()
        {
            // Arrange
            SetupUserNotFound();

            var input = new AllWarehousesSearchFilterViewModel();

            // Act
            var result = await warehouseService.GetWarehousesForUserAsync(input, userId);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task FiltersBySearchQuery()
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
        public async Task FiltersBySingleYear()
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
        public async Task FiltersByYearRange()
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
        public async Task RespectsPagination()
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
        public async Task PaginationValidation(
            int entitiesPerPageInput,
            int currentPageInput,
            int expectedEntitiesPerPage,
            int expectedCurrentPage)
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
        public async Task ReturnsFailureOnException()
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

            logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(RetrievingFailure)),
                It.Is<Exception>(ex => ex.Message == "Simulated failure"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
