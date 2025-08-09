using Microsoft.Extensions.Logging;
using Moq;

using WarehouseApp.Data.Models;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

namespace WarehouseApp.Services.Tests.WarehouseServiceTests
{
    [TestFixture]
    public class MarkAsDeletedWithoutSavingAsyncTests : WarehouseServiceBaseTests
    {
        [Test]
        public async Task ShouldReturnFailure_WhenWarehouseNotFound()
        {
            warehouseRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                         .ReturnsAsync((Warehouse?)null);

            var result = await warehouseService.MarkAsDeletedWithoutSavingAsync(Guid.NewGuid());

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(WarehouseNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenWarehouseAlreadyDeleted()
        {
            var warehouse = warehousesList[0];
            warehouse.IsDeleted = true;

            warehouseRepo.Setup(r => r.GetByIdAsync(warehouse.Id))
                         .ReturnsAsync(warehouse);

            var result = await warehouseService.MarkAsDeletedWithoutSavingAsync(warehouse.Id);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(AlreadyDeleted));
        }

        [Test]
        public async Task ShouldReturnOk_WhenWarehouseMarkedAsDeletedSuccessfully()
        {
            var warehouse = warehousesList[0];
            warehouse.IsDeleted = false;

            warehouseRepo.Setup(r => r.GetByIdAsync(warehouse.Id))
                         .ReturnsAsync(warehouse);

            var result = await warehouseService.MarkAsDeletedWithoutSavingAsync(warehouse.Id);

            Assert.That(result.Success, Is.True);
            Assert.That(warehouse.IsDeleted, Is.True);
            Assert.That(warehouse.Name, Does.Contain("DeletedOn"));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExceptionIsThrown()
        {
            warehouseRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                         .ThrowsAsync(new Exception("Unexpected"));

            var result = await warehouseService.MarkAsDeletedWithoutSavingAsync(Guid.NewGuid());

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(DeletionFailure));

            logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(DeletionFailure)),
                It.Is<Exception>(ex => ex.Message == "Unexpected"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
