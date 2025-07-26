using Moq;

using WarehouseApp.Data.Models;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

namespace WarehouseApp.Services.Tests.WarehouseServiceTests
{
    [TestFixture]
    public class DeleteWarehouseAsyncTests : WarehouseServiceBaseTests
    {
        [Test]
        public async Task ShouldReturnFailure_WhenUserNotFound()
        {
            SetupUserNotFound();

            var result = await warehouseService.DeleteWarehouseAsync(Guid.NewGuid(), userId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenWarehouseNotFound()
        {
            warehouseRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                         .ReturnsAsync((Warehouse?)null);

            var result = await warehouseService.DeleteWarehouseAsync(Guid.NewGuid(), userId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(WarehouseNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenWarehouseIsAlreadyDeleted()
        {
            var warehouse = warehousesList[0];
            warehouse.IsDeleted = true;

            warehouseRepo.Setup(x => x.GetByIdAsync(warehouse.Id))
                         .ReturnsAsync(warehouse);

            var result = await warehouseService.DeleteWarehouseAsync(warehouse.Id, userId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(AlreadyDeleted));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenUserHasNoPermission()
        {
            var warehouse = warehousesList[0];
            warehouse.IsDeleted = false;
            warehouse.CreatedByUserId = Guid.NewGuid();

            warehouseRepo.Setup(x => x.GetByIdAsync(warehouse.Id))
                         .ReturnsAsync(warehouse);

            var result = await warehouseService.DeleteWarehouseAsync(warehouse.Id, userId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermission));
        }

        [Test]
        public async Task ShouldReturnOk_WhenWarehouseIsDeletedSuccessfully()
        {
            var warehouse = warehousesList[0];
            warehouse.IsDeleted = false;
            warehouse.CreatedByUserId = userId;

            warehouseRepo.Setup(x => x.GetByIdAsync(warehouse.Id))
                         .ReturnsAsync(warehouse);

            warehouseRepo.Setup(x => x.SaveChangesAsync())
                         .Returns(Task.CompletedTask);

            var result = await warehouseService.DeleteWarehouseAsync(warehouse.Id, userId);

            Assert.That(result.Success, Is.True);
            Assert.That(warehouse.IsDeleted, Is.True);
            Assert.That(warehouse.Name, Does.Contain("DeletedOn"));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExceptionIsThrown()
        {
            warehouseRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                         .ThrowsAsync(new Exception("DB down"));

            var result = await warehouseService.DeleteWarehouseAsync(Guid.NewGuid(), userId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(DeletionFailure));
        }
    }
}
