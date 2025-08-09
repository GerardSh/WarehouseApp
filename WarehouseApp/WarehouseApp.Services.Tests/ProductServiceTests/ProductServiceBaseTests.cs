using Moq;
using Microsoft.Extensions.Logging;

using WarehouseApp.Data.Repository.Interfaces;

namespace WarehouseApp.Services.Tests.ProductServiceTests
{
    [TestFixture]
    public abstract class ProductServiceBaseTests
    {
        protected Mock<IProductRepository> productRepo;
        protected Mock<ILogger<ProductService>> logger;

        protected ProductService productService;

        [SetUp]
        public void Setup()
        {
            productRepo = new Mock<IProductRepository>();

            logger = new Mock<ILogger<ProductService>>();

            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));

            productService = new ProductService(productRepo.Object, logger.Object);
        }
    }
}
