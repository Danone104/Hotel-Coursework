using FluentAssertions;
using Hotel.DataAccess.DBAdapter;
using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using Hotel.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Tests.IntegrationTests;
using Xunit;

namespace Hotel.IntegrationTests
{
    public class DbContextTests :BaseTest   
    {
        [Fact]
        public async Task CanAddAndRetrieveUser()
        {
            var user = new User("hashedPass", "TestUser", Roles.Admin);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var retrievedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

            retrievedUser.Should().NotBeNull();
            retrievedUser!.UserName.Should().Be("TestUser");
            retrievedUser.Role.Should().Be(Roles.Admin);
        }

        [Fact]
        public async Task CanAddAndRetrieveClientWithComplexTypes()
        {
            var phone = new PhoneNumber("+71234567890");
            var name = new FullName("John", "Doe", "Smith"); 
            var client = new Client(phone, name);

            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var retrievedClient = await _context.Clients.FirstOrDefaultAsync(c => c.Id == client.Id);

            retrievedClient.Should().NotBeNull();
            retrievedClient!.TelephoneNum.Value.Should().Be("+71234567890");
            retrievedClient.Name.FirstName.Should().Be("John");
            retrievedClient.Name.LastName.Should().Be("Doe");
            retrievedClient.Name.Patronymic.Should().Be("Smith");
        }

        [Fact]
        public async Task BookingHasCorrectRelationships()
        {
            var phone = new PhoneNumber("+71234567890");
            var name = new FullName("John", "Doe", "Smith");
            var client = new Client(phone, name);

            var room = new Room("Deluxe", 2, "Nice room", 150m);

            await _context.Clients.AddAsync(client);
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var booking = new Booking(client.Id, room.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                150m);

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var retrievedBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == booking.Id);

            retrievedBooking.Should().NotBeNull();
            retrievedBooking!.ClientId.Should().Be(client.Id);
            retrievedBooking.RoomId.Should().Be(room.Id);
        }

        [Fact]
        public async Task RentRoomHasOneToOneWithBooking()
        {
            var phone = new PhoneNumber("+71234567890");
            var name = new FullName("John", "Doe", null);
            var client = new Client(phone, name);

            var room = new Room("Standard", 2, "Room", 100m);

            await _context.Clients.AddAsync(client);
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var booking = new Booking(client.Id, room.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 100m);
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var rent = new RentRoom(booking.Id);

            await _context.Rents.AddAsync(rent);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var retrievedRent = await _context.Rents.FirstOrDefaultAsync(r => r.Id == rent.Id);

            retrievedRent.Should().NotBeNull();
            retrievedRent!.BookId.Should().Be(booking.Id);
        }

        [Fact]
        public async Task RoomStatusConvertsToString()
        {
            var room = new Room("Test", 2, "Test", 100m);
            room.DeleteRoom();

            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var retrievedRoom = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == room.Id);

            retrievedRoom.Should().NotBeNull();
            retrievedRoom!.Status.Should().Be(RoomStatus.Deleted);
        }
    }
}