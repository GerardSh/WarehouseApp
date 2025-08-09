using Moq;
using Microsoft.Extensions.Logging;

using WarehouseApp.Data.Repository.Interfaces;

namespace WarehouseApp.Services.Tests.CategoryServiceTests
{
    [TestFixture]
    public abstract class CategoryServiceBaseTests
    {
        protected Mock<ICategoryRepository> categoryRepo;

        protected Mock<ILogger<CategoryService>> logger;

        protected CategoryService categoryService;

        [SetUp]
        public void Setup()
        {
            categoryRepo = new Mock<ICategoryRepository>();

            logger = new Mock<ILogger<CategoryService>>();

            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));

            categoryService = new CategoryService(categoryRepo.Object, logger.Object);
        }
    }
}
