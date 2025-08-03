using MockQueryable;
using Moq;

using WarehouseApp.Data.Models;

namespace WarehouseApp.Services.Tests.ProductServiceTests
{
    [TestFixture]
    public class GetOrCreateOrUpdateProductAsyncTests : ProductServiceBaseTests
    {
        private readonly Guid categoryId = Guid.NewGuid();
        private string name = "Product A";
        private string desc = "Description A";
        private Guid id = Guid.NewGuid();
        private Product product;

        [SetUp]
        public void TestSetup()
        {
            product = new Product { Name = name, Description = desc, CategoryId = categoryId, Id = id };

            productRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        }

        [Test]
        public async Task CreatesNewProduct_WhenNoneExists()
        {
            // Arrange
            productRepo.Setup(r => r.AllTracked())
                .Returns(new List<Product>().AsQueryable().BuildMock());

            // Act
            var result = await productService.GetOrCreateOrUpdateProductAsync(name, desc, categoryId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.Name, Is.EqualTo(name));
            Assert.That(result.Data.Description, Is.EqualTo(desc));
            Assert.That(result.Data.CategoryId, Is.EqualTo(categoryId));
            productRepo.Verify(r => r.AddAsync(It.Is<Product>(
                p => p.Name == name && p.Description == desc && p.CategoryId == categoryId)), Times.Once);
        }

        [Test]
        public async Task ReturnsExistingProduct_WhenMatchFound_WithoutChanges()
        {
            // Arrange
            productRepo.Setup(r => r.AllTracked())
                .Returns(new List<Product> { product }.AsQueryable().BuildMock());

            // Act
            var result = await productService.GetOrCreateOrUpdateProductAsync(name, desc, categoryId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.SameAs(product));
            productRepo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
        }

        [Test]
        public async Task UpdatesDescription_WhenDifferent()
        {
            // Arrange
            var newDesc = "New Description";

            productRepo.Setup(r => r.AllTracked())
                .Returns(new List<Product> { product }.AsQueryable().BuildMock());

            // Act
            var result = await productService.GetOrCreateOrUpdateProductAsync(name, newDesc, categoryId);

            // Assert
            Assert.That(result.Data!.Description, Is.EqualTo(newDesc));
            Assert.That(result.Data!.Name, Is.EqualTo(name));
            Assert.That(result.Data!.CategoryId, Is.EqualTo(categoryId));
            Assert.That(result.Data!.Id, Is.EqualTo(id));
            productRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [TestCase(null)]
        [TestCase("Description A")]
        public async Task SkipsUpdate_WhenDescriptionIsNullOrSame(string? incomingDescription)
        {
            // Arrange
            productRepo.Setup(r => r.AllTracked())
                .Returns(new List<Product> { product }.AsQueryable().BuildMock());

            // Act
            var result = await productService.GetOrCreateOrUpdateProductAsync(name, incomingDescription, categoryId);

            // Assert
            Assert.That(result.Data!.Description, Is.EqualTo(desc));
            Assert.That(result.Data!.Name, Is.EqualTo(name));
            Assert.That(result.Data!.CategoryId, Is.EqualTo(categoryId));
            Assert.That(result.Data!.Id, Is.EqualTo(id));
            Assert.That(result.Data, Is.SameAs(product));

            productRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
