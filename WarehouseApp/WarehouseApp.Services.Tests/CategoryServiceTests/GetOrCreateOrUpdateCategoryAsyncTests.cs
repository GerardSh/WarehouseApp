using MockQueryable;
using Moq;

using WarehouseApp.Data.Models;

namespace WarehouseApp.Services.Tests.CategoryServiceTests
{
    [TestFixture]
    public class GetOrCreateOrUpdateCategoryAsyncTests : CategoryServiceBaseTests
    {
        private string name = "Category A";
        private string desc = "Description A";
        private Guid id = Guid.NewGuid();
        private Category category;

        [SetUp]
        public void TestSetup()
        {
            category = new Category { Name = name, Description = desc, Id = id };

            categoryRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        }

        [Test]
        public async Task CreatesNewCategory_WhenNoneExists()
        {
            // Arrange
            categoryRepo.Setup(r => r.GetTrackedLocal(It.IsAny<Func<Category, bool>>()))
                        .Returns((Category)null!);

            categoryRepo.Setup(r => r.AllTracked())
                .Returns(new List<Category>().AsQueryable().BuildMock());

            // Act
            var result = await categoryService.GetOrCreateOrUpdateCategoryAsync(name, desc);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.Name, Is.EqualTo(name));
            Assert.That(result.Data.Description, Is.EqualTo(desc));
            categoryRepo.Verify(r => r.AddAsync(It.Is<Category>(
                c => c.Name == name && c.Description == desc)), Times.Once);
        }

        [Test]
        public async Task ReturnsTrackedLocalCategory_IfAlreadyTracked()
        {
            // Arrange
            categoryRepo.Setup(r => r.GetTrackedLocal(It.IsAny<Func<Category, bool>>()))
                        .Returns(category);

            // Act
            var result = await categoryService.GetOrCreateOrUpdateCategoryAsync(name, desc);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.SameAs(category));
            categoryRepo.Verify(r => r.GetTrackedLocal(It.IsAny<Func<Category, bool>>()), Times.Once);
            categoryRepo.Verify(r => r.AllTracked(), Times.Never);
            categoryRepo.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Never);
        }

        [Test]
        public async Task ReturnsExistingCategory_WhenMatchFound_WithoutChanges()
        {
            // Arrange
            categoryRepo.Setup(r => r.GetTrackedLocal(It.IsAny<Func<Category, bool>>()))
                        .Returns((Category)null!);

            categoryRepo.Setup(r => r.AllTracked())
                .Returns(new List<Category> { category }.AsQueryable().BuildMock());

            // Act
            var result = await categoryService.GetOrCreateOrUpdateCategoryAsync(name, desc);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.SameAs(category));
            categoryRepo.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Never);
        }

        [Test]
        public async Task UpdatesDescription_WhenDifferent()
        {
            // Arrange
            var newDesc = "New Description";

            categoryRepo.Setup(r => r.GetTrackedLocal(It.IsAny<Func<Category, bool>>()))
                        .Returns((Category)null!);

            categoryRepo.Setup(r => r.AllTracked())
                .Returns(new List<Category> { category }.AsQueryable().BuildMock());

            // Act
            var result = await categoryService.GetOrCreateOrUpdateCategoryAsync(name, newDesc);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.Description, Is.EqualTo(newDesc));
            Assert.That(result.Data!.Name, Is.EqualTo(name));
            Assert.That(result.Data!.Id, Is.EqualTo(id));
            categoryRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [TestCase(null)]
        [TestCase("Description A")]
        public async Task SkipsUpdate_WhenDescriptionIsNullOrSame(string? incomingDescription)
        {
            // Arrange
            categoryRepo.Setup(r => r.GetTrackedLocal(It.IsAny<Func<Category, bool>>()))
                        .Returns((Category)null!);

            categoryRepo.Setup(r => r.AllTracked())
                .Returns(new List<Category> { category }.AsQueryable().BuildMock());

            // Act
            var result = await categoryService.GetOrCreateOrUpdateCategoryAsync(name, incomingDescription);

            // Assert
            Assert.That(result.Data!.Description, Is.EqualTo(desc));
            Assert.That(result.Data!.Name, Is.EqualTo(name));
            Assert.That(result.Data!.Id, Is.EqualTo(id));
            categoryRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
