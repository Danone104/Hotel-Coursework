using FluentAssertions;
using Hotel.DataAccess.Repositories;
using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using Hotel.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Tests.IntegrationTests;
using Xunit;

namespace Hotel.IntegrationTests.Repositories
{
    public class ClientRepositoryTests : BaseTest
    {
        private readonly ClientRepository _repository;

        public ClientRepositoryTests()
        {
            _repository = new ClientRepository(_context);
        }

        private async Task<User> CreateTestUser()
        {
            var user = new User("hashedPassword", "TestUser", Roles.Client);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }


        private Client CreateValidClient(bool withUserId = false)
        {
            var phone = new PhoneNumber("+79234567890");
            var name = new FullName("John", "Doe",null);

            if (withUserId)
            {
                return new Client(Guid.NewGuid(), phone, name);
            }
            return new Client(phone, name);
        }


        [Fact]
        public async Task AddClient_ValidClient_ClientAdded()
        {
            var client = CreateValidClient();

            await _repository.AddClient(client);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var result = await _repository.GetClientById(client.Id);

            result.Should().NotBeNull();
            result!.Name.FirstName.Should().Be("John");
            result.Name.LastName.Should().Be("Doe");
            result.TelephoneNum.Value.Should().Be("+79234567890");
        }

        [Fact]
        public async Task GetClientById_ExistingClient_ReturnsClient()
        {
            var client = CreateValidClient();
            await _repository.AddClient(client);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var result = await _repository.GetClientById(client.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(client.Id);
        }

        [Fact]
        public async Task GetByNameAndPhoneAsync_ExistingClient_ReturnsClient()
        {
            var phone = new PhoneNumber("+79234567890");
            var name = new FullName("Jane", "Smith",null);
            var client = new Client(phone, name);

            await _repository.AddClient(client);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var result = await _repository.GetByNameAndPhoneAsync(name, phone);

            result.Should().NotBeNull();
            result!.Id.Should().Be(client.Id);
        }

        [Fact]
        public async Task GetClientByUserId_ExistingUser_ReturnsClient()
        {
            var user = new User("hashedPassword", "TestUser", Roles.Client);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync(); 

            _context.ChangeTracker.Clear();

            var phone = new PhoneNumber("+79234567890");
            var name = new FullName("John", "Doe", null);
            var client = new Client(user.Id, phone, name);

            await _repository.AddClient(client);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var result = await _repository.GetClientByUserId(user.Id);

            result.Should().NotBeNull();
            result!.UserId.Should().Be(user.Id);
        }

        [Fact]
        public async Task GetClientByUserId_NonExistingUser_ReturnsNull()
        {
            var client = CreateValidClient();
            await _repository.AddClient(client);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var result = await _repository.GetClientByUserId(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetClientsByBookingsAsync_WithValidBookings_ReturnsUniqueClients()
        {
            var client1 = new Client(new PhoneNumber("+79234567890"), new FullName("Client1", "One", null));
            var client2 = new Client(new PhoneNumber("+79234567891"), new FullName("Client2", "Two", null));

            await _repository.AddClient(client1);
            await _repository.AddClient(client2);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var room = new Room("Test Room", 2, "Test", 100m);
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var booking1 = new Booking(client1.Id, room.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 100m);
            var booking2 = new Booking(client1.Id, room.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(4)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(6)), 100m);
            var booking3 = new Booking(client2.Id, room.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 100m);

            await _context.Bookings.AddRangeAsync(booking1, booking2, booking3);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var bookings = new List<Booking> { booking1, booking2, booking3 };

            var clients = await _repository.GetClientsByBookingsAsync(bookings);

            clients.Should().HaveCount(2);
            clients.Select(c => c.Id).Should().Contain(client1.Id);
            clients.Select(c => c.Id).Should().Contain(client2.Id);
        }

        [Fact]
        public async Task GetClientsByBookingsAsync_WithEmptyBookingsList_ReturnsEmptyList()
        {
            var client = CreateValidClient();
            await _repository.AddClient(client);
            await _context.SaveChangesAsync();

            var emptyBookings = new List<Booking>();

            var clients = await _repository.GetClientsByBookingsAsync(emptyBookings);

            clients.Should().BeEmpty();
        }

        [Fact]
        public async Task GetClientsByBookingsAsync_WithNoMatchingBookings_ReturnsEmptyList()
        {
            var client = CreateValidClient();
            await _repository.AddClient(client);
            await _context.SaveChangesAsync();

            var room = new Room("Test Room", 2, "Test", 100m);
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            var booking = new Booking(Guid.NewGuid(), room.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 100m);

            var bookings = new List<Booking> { booking };

            var clients = await _repository.GetClientsByBookingsAsync(bookings);

            clients.Should().BeEmpty();
        }

        [Fact]
        public async Task LinkToUser_ClientWithoutUser_UpdatesSuccessfully()
        {
            var user = new User("hashedPassword", "NewUser", Roles.Client);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var phone = new PhoneNumber("+79234567890");
            var name = new FullName("John", "Doe",null);
            var client = new Client(phone, name);

            await _repository.AddClient(client);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var clientToUpdate = await _repository.GetClientById(client.Id);
            clientToUpdate!.LinkToUser(user.Id);

            _context.Entry(clientToUpdate).Property(x => x.UserId).IsModified = true;

            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var result = await _repository.GetClientById(client.Id);
            result.Should().NotBeNull();
            result!.UserId.Should().Be(user.Id);
        }

        [Fact]
        public async Task LinkToUser_ClientAlreadyHasUser_ThrowsException()
        {
            var user1 = await CreateTestUser();
            var user2 = await CreateTestUser(); 

            var phone = new PhoneNumber("+71234567890");
            var name = new FullName("John", "Doe",null);
            var client = new Client(user1.Id, phone, name);

            await _repository.AddClient(client);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var clientToUpdate = await _repository.GetClientById(client.Id);

            Assert.Throws<ArgumentException>(() => clientToUpdate!.LinkToUser(user2.Id));
        }
    }
}