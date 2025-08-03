using Moq;

using WarehouseApp.Data.Repository.Interfaces;

namespace WarehouseApp.Services.Tests.CategoryServiceTests
{
    [TestFixture]
    public abstract class CategoryServiceBaseTests
    {
        protected Mock<ICategoryRepository> categoryRepo;

        protected CategoryService categoryService;

        [SetUp]
        public void Setup()
        {
            categoryRepo = new Mock<ICategoryRepository>();
            categoryService = new CategoryService(categoryRepo.Object);
        }
    }
}
