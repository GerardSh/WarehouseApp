using Moq;

using WarehouseApp.Data.Repository.Interfaces;

namespace WarehouseApp.Services.Tests.ClientServiceTests
{
    [TestFixture]
    public abstract class ClientServiceBaseTests
    {
        protected Mock<IClientRepository> clientRepo;

        protected ClientService clientService;

        [SetUp]
        public void Setup()
        {
            clientRepo = new Mock<IClientRepository>();
            clientService = new ClientService(clientRepo.Object);
        }
    }
}
