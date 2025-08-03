using Moq;

using WarehouseApp.Data.Repository.Interfaces;

namespace WarehouseApp.Services.Tests.ProductServiceTests
{
    [TestFixture]
    public abstract class ProductServiceBaseTests
    {
        protected Mock<IProductRepository> productRepo;

        protected ProductService productService;

        [SetUp]
        public void Setup()
        {
            productRepo = new Mock<IProductRepository>();
            productService = new ProductService(productRepo.Object);
        }
    }
}
