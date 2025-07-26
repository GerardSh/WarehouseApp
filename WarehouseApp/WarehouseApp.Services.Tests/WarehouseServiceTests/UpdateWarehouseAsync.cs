using Moq;

using WarehouseApp.Data.Models;
using WarehouseApp.Web.ViewModels.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

namespace WarehouseApp.Services.Tests.WarehouseServiceTests
{
    [TestFixture]
    public class UpdateWarehouseAsyncTests : WarehouseServiceBaseTests
    {
        [Test]
        public async Task ShouldReturnFailure_WhenUserNotFound()
        {
            SetupUserNotFound();

            var inputModel = new EditWarehouseInputModel { Id = Guid.NewGuid() };

            var result = await warehouseService.UpdateWarehouseAsync(inputModel, userId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(UserNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenWarehouseNotFound()
        {
            warehouseRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                         .ReturnsAsync((Warehouse?)null);

            var inputModel = new EditWarehouseInputModel { Id = Guid.NewGuid() };

            var result = await warehouseService.UpdateWarehouseAsync(inputModel, userId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(WarehouseNotFound));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenUserHasNoPermission()
        {
            var warehouse = warehousesList[0];
            warehouse.CreatedByUserId = Guid.NewGuid();

            warehouseRepo.Setup(x => x.GetByIdAsync(warehouse.Id))
                         .ReturnsAsync(warehouse);

            var inputModel = new EditWarehouseInputModel { Id = warehouse.Id };

            var result = await warehouseService.UpdateWarehouseAsync(inputModel, userId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(NoPermission));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenDuplicateNameExists()
        {
            var warehouse = warehousesList[0];
            warehouse.CreatedByUserId = userId;

            warehouseRepo.Setup(x => x.GetByIdAsync(warehouse.Id))
                         .ReturnsAsync(warehouse);

            appUserWarehouseRepo.Setup(x => x.UserHasWarehouseWithNameAsync(userId, It.IsAny<string>(), warehouse.Id))
                                .ReturnsAsync(true);

            var inputModel = new EditWarehouseInputModel
            {
                Id = warehouse.Id,
                Name = "ExistingName",
                Address = "New Address",
                Size = 100
            };

            var result = await warehouseService.UpdateWarehouseAsync(inputModel, userId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(WarehouseDuplicateName));
        }

        [Test]
        public async Task ShouldReturnOk_WhenWarehouseUpdatedSuccessfully()
        {
            var warehouse = warehousesList[0];
            warehouse.CreatedByUserId = userId;

            warehouseRepo.Setup(x => x.GetByIdAsync(warehouse.Id))
                         .ReturnsAsync(warehouse);

            appUserWarehouseRepo.Setup(x => x.UserHasWarehouseWithNameAsync(userId, It.IsAny<string>(), warehouse.Id))
                                .ReturnsAsync(false);

            warehouseRepo.Setup(x => x.SaveChangesAsync())
                         .Returns(Task.CompletedTask);

            var inputModel = new EditWarehouseInputModel
            {
                Id = warehouse.Id,
                Name = "UpdatedName",
                Address = "Updated Address",
                Size = 300
            };

            var result = await warehouseService.UpdateWarehouseAsync(inputModel, userId);

            Assert.That(result.Success, Is.True);
            Assert.That(warehouse.Name, Is.EqualTo("UpdatedName"));
            Assert.That(warehouse.Address, Is.EqualTo("Updated Address"));
            Assert.That(warehouse.Size, Is.EqualTo(300));
        }

        [Test]
        public async Task ShouldReturnFailure_WhenExceptionIsThrown()
        {
            warehouseRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                         .ThrowsAsync(new Exception("Unexpected error"));

            var inputModel = new EditWarehouseInputModel { Id = Guid.NewGuid() };

            var result = await warehouseService.UpdateWarehouseAsync(inputModel, userId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(EditingFailure));
        }
    }
}