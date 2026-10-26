using FluentAssertions;
using Hotel.Application.Services;
using Hotel.Domain.Abstractions.Repositories;
using Hotel.Domain.Models;
using Hotel.Domain.ValueObjects;
using Moq;
using Xunit;

namespace Hotel.Application.Tests.Services
{
    public class ClientServiceTests
    {
        private readonly Mock<IClientRepository> _clientRepoMock;
        private readonly ClientService _service;

        public ClientServiceTests()
        {
            _clientRepoMock = new Mock<IClientRepository>();
            _service = new ClientService(_clientRepoMock.Object);
        }

        [Fact]
        public async Task GetClientByIdAsync_ExistingClient_ReturnsClient()
        {
            var clientId = Guid.NewGuid();
            var expectedClient = CreateTestClient(clientId);

            _clientRepoMock.Setup(x => x.GetClientById(clientId))
                .ReturnsAsync(expectedClient);

            var result = await _service.GetClientByIdAsync(clientId);

            result.Should().NotBeNull();
            result.Id.Should().Be(clientId);
            result.Name.FirstName.Should().Be("John");
            result.Name.LastName.Should().Be("Doe");
            _clientRepoMock.Verify(x => x.GetClientById(clientId), Times.Once);
        }

        [Fact]
        public async Task GetClientByIdAsync_NonExistingClient_ThrowsArgumentException()
        {
            var clientId = Guid.NewGuid();
            _clientRepoMock.Setup(x => x.GetClientById(clientId))
                .ReturnsAsync((Client?)null);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.GetClientByIdAsync(clientId));
        }

        [Fact]
        public async Task GetClientByUserIdAsync_NonExistingUser_ThrowsArgumentException()
        {
            var userId = Guid.NewGuid();
            _clientRepoMock.Setup(x => x.GetClientByUserId(userId))
                .ReturnsAsync((Client?)null);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.GetClientByUserIdAsync(userId));

            exception.Message.Should().Contain($"Пользователь с id {userId} не зарегистрирован");
        }

        [Fact]
        public async Task GetClientsForBookingsAsync_WithValidBookings_ReturnsClients()
        {
            var client1 = CreateTestClient(Guid.NewGuid());
            var client2 = CreateTestClient(Guid.NewGuid());
            var expectedClients = new List<Client> { client1, client2 };

            var bookings = new List<Booking>
            {
                new Booking(client1.Id, Guid.NewGuid(),
                    DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 100m),
                new Booking(client2.Id, Guid.NewGuid(),
                    DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 100m)
            };

            _clientRepoMock.Setup(x => x.GetClientsByBookingsAsync(bookings))
                .ReturnsAsync(expectedClients);

            var result = await _service.GetClientsForBookingsAsync(bookings);

            result.Should().HaveCount(2);
            result.Should().Contain(client1);
            result.Should().Contain(client2);
            _clientRepoMock.Verify(x => x.GetClientsByBookingsAsync(bookings), Times.Once);
        }

        [Fact]
        public async Task GetClientsForBookingsAsync_WithEmptyBookings_ReturnsEmptyList()
        {
            var emptyBookings = new List<Booking>();
            _clientRepoMock.Setup(x => x.GetClientsByBookingsAsync(emptyBookings))
                .ReturnsAsync(new List<Client>());

            var result = await _service.GetClientsForBookingsAsync(emptyBookings);

            result.Should().BeEmpty();
            _clientRepoMock.Verify(x => x.GetClientsByBookingsAsync(emptyBookings), Times.Once);
        }

        private Client CreateTestClient(Guid clientId, Guid? userId = null)
        {
            var phone = new PhoneNumber("+71234567890");
            var name = new FullName("John", "Doe",null);

            if (userId.HasValue)
            {
                return new Client(userId.Value, phone, name);
            }

            var client = new Client(phone, name);
            var idField = typeof(Client).GetProperty("Id");
            if (idField != null && idField.CanWrite)
            {
                idField.SetValue(client, clientId);
            }

            return client;
        }
    }
}