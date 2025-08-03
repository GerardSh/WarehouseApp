using MockQueryable;
using Moq;

using WarehouseApp.Data.Models;

namespace WarehouseApp.Services.Tests.ClientServiceTests
{
    [TestFixture]
    public class GetOrCreateOrUpdateClientAsyncTests : ClientServiceBaseTests
    {
        private string name = "Product A";
        private string address = "Address A";
        private string phone = "0888888888";
        private string email = "test@email.com";
        private Guid id = Guid.NewGuid();
        private Client client;

        [SetUp]
        public void TestSetup()
        {
            client = new Client
            {
                Name = name,
                Address = address,
                PhoneNumber = phone,
                Email = email,
                Id = id
            };

            clientRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        }

        [Test]
        public async Task CreatesNewClient_WhenNoneExists()
        {
            // Arrange
            clientRepo.Setup(r => r.AllTracked())
                      .Returns(new List<Client>().AsQueryable().BuildMock());

            // Act
            var result = await clientService.GetOrCreateOrUpdateClientAsync(name, address, phone, email);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.Name, Is.EqualTo(name));
            Assert.That(result.Data.Address, Is.EqualTo(address));
            Assert.That(result.Data.PhoneNumber, Is.EqualTo(phone));
            Assert.That(result.Data.Email, Is.EqualTo(email));
            clientRepo.Verify(r => r.AddAsync(It.Is<Client>(
                c => c.Name == name && c.Address == address &&
                     c.PhoneNumber == phone && c.Email == email)), Times.Once);
        }

        [Test]
        public async Task ReturnsExistingClient_WhenMatchFound_WithoutChanges()
        {
            // Arrange
            clientRepo.Setup(r => r.AllTracked())
                      .Returns(new List<Client> { client }.AsQueryable().BuildMock());

            // Act
            var result = await clientService.GetOrCreateOrUpdateClientAsync(name, address, phone, email);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.SameAs(client));
            Assert.That(result.Data!.Name, Is.EqualTo(name));
            Assert.That(result.Data.Address, Is.EqualTo(address));
            Assert.That(result.Data.PhoneNumber, Is.EqualTo(phone));
            Assert.That(result.Data.Email, Is.EqualTo(email));
            Assert.That(result.Data.Id, Is.EqualTo(id));
            clientRepo.Verify(r => r.AddAsync(It.IsAny<Client>()), Times.Never);
        }

        [Test]
        public async Task UpdatesClient_WhenFieldsAreDifferent()
        {
            // Arrange
            var newAddress = "New Address";
            var newPhone = "0999999999";
            var newEmail = "new@email.com";

            clientRepo.Setup(r => r.AllTracked())
                      .Returns(new List<Client> { client }.AsQueryable().BuildMock());

            // Act
            var result = await clientService.GetOrCreateOrUpdateClientAsync(name, newAddress, newPhone, newEmail);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.Address, Is.EqualTo(newAddress));
            Assert.That(result.Data!.PhoneNumber, Is.EqualTo(newPhone));
            Assert.That(result.Data!.Email, Is.EqualTo(newEmail));
            Assert.That(result.Data!.Id, Is.EqualTo(id));
            clientRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [TestCase(null, null, null)]
        [TestCase("", "", "")]
        [TestCase("   ", "   ", "   ")]
        public async Task DoesNotOverwrite_WhenEmptyOrNullValuesArePassed(string? addr, string? phoneNum, string? mail)
        {
            // Arrange
            clientRepo.Setup(r => r.AllTracked())
                      .Returns(new List<Client> { client }.AsQueryable().BuildMock());

            // Act
            var result = await clientService.GetOrCreateOrUpdateClientAsync(name, addr, phoneNum, mail);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.Address, Is.EqualTo(address));
            Assert.That(result.Data!.PhoneNumber, Is.EqualTo(phone));
            Assert.That(result.Data!.Email, Is.EqualTo(email));
            clientRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task UpdatesClient_WhenOnlyCasingIsDifferent()
        {
            // Arrange
            client = new Client
            {
                Id = id,
                Name = "john doe",
                Address = "old address",
                PhoneNumber = "0888888888",
                Email = "email@old.com"
            };

            clientRepo.Setup(r => r.AllTracked())
                      .Returns(new List<Client> { client }.AsQueryable().BuildMock());

            string newName = "John Doe";
            string newAddress = "Old Address";
            string newPhone = "0888888888";
            string newEmail = "Email@Old.com";

            // Act
            var result = await clientService.GetOrCreateOrUpdateClientAsync(
                newName, newAddress, newPhone, newEmail);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.Name, Is.EqualTo(newName));
            Assert.That(result.Data.Address, Is.EqualTo(newAddress));
            Assert.That(result.Data.PhoneNumber, Is.EqualTo(newPhone));
            Assert.That(result.Data.Email, Is.EqualTo(newEmail));
            Assert.That(result.Data.Id, Is.EqualTo(id));
            clientRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task OverwritesOnlyProvidedFields()
        {
            // Arrange
            var newEmail = "updated@email.com";

            clientRepo.Setup(r => r.AllTracked())
                      .Returns(new List<Client> { client }.AsQueryable().BuildMock());

            // Act
            var result = await clientService.GetOrCreateOrUpdateClientAsync(name, null, null, newEmail);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data!.Address, Is.EqualTo(address));
            Assert.That(result.Data!.PhoneNumber, Is.EqualTo(phone));
            Assert.That(result.Data!.Email, Is.EqualTo(newEmail));
            clientRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
