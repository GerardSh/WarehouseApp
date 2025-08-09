using Moq;
using Microsoft.Extensions.Logging;

using WarehouseApp.Data.Repository.Interfaces;

namespace WarehouseApp.Services.Tests.ClientServiceTests
{
    [TestFixture]
    public abstract class ClientServiceBaseTests
    {
        protected Mock<IClientRepository> clientRepo;

        protected Mock<ILogger<ClientService>> logger;

        protected ClientService clientService;

        [SetUp]
        public void Setup()
        {
            clientRepo = new Mock<IClientRepository>();

            logger = new Mock<ILogger<ClientService>>();

            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));

            clientService = new ClientService(clientRepo.Object, logger.Object);
        }
    }
}
