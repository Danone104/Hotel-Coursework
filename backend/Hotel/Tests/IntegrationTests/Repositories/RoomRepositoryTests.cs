using FluentAssertions;
using Hotel.DataAccess.Repositories;
using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Tests.IntegrationTests;
using Xunit;

namespace Hotel.IntegrationTests.Repositories
{
    public class RoomRepositoryTests : BaseTest
    {
        private readonly RoomRepository _repository;

        public RoomRepositoryTests()
        {
            _repository = new RoomRepository(_context);
        }

        [Fact]
        public async Task AddRoom_ValidRoom_RoomAdded()
        {   
            var room = new Room("Suite", 4, "Luxury suite", 250m);

            await _repository.AddRoom(room);
            await _repository.SaveChangesAsync();
            var result = await _repository.GetRoomById(room.Id);

            result.Should().NotBeNull();
            result!.Title.Should().Be("Suite");
            result.Status.Should().Be(RoomStatus.Ready);
        }

        [Fact]
        public async Task GetRooms_ReturnsOnlyNonDeletedRooms()
        {
            var room1 = new Room("Room1", 2, "Standard", 100m);
            var room2 = new Room("Room2", 3, "Deluxe", 150m);
            room2.DeleteRoom();

            await _repository.AddRoom(room1);
            await _repository.AddRoom(room2);
            await _repository.SaveChangesAsync();

            var rooms = await _repository.GetRooms();

            rooms.Should().HaveCount(1);
            rooms[0].Title.Should().Be("Room1");
        }

        [Fact]
        public async Task GetRoomsByPage_ReturnsCorrectPage()
        {
            for (int i = 1; i <= 10; i++)
            {
                var room = new Room($"Room{i}", 2, "Standard", 100m);
                await _repository.AddRoom(room);
            }
            await _repository.SaveChangesAsync();

            var page1 = await _repository.GetRoomsByPage(1, 3);
            var page2 = await _repository.GetRoomsByPage(2, 3);

            page1.Should().HaveCount(3);
            page2.Should().HaveCount(3);
            page1.Select(r => r.Id).Should().NotIntersectWith(page2.Select(r => r.Id));
        }

        [Fact]
        public async Task GetRoomsByBookingsAsync_ReturnsCorrectRooms()
        {
            var room1 = new Room("Room1", 2, "Standard", 100m);
            var room2 = new Room("Room2", 3, "Deluxe", 150m);
            await _repository.AddRoom(room1);
            await _repository.AddRoom(room2);
            await _repository.SaveChangesAsync();

            var booking1 = new Booking(Guid.NewGuid(), room1.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 100m);
            var booking2 = new Booking(Guid.NewGuid(), room1.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(7)), 100m);

            var bookings = new List<Booking> { booking1, booking2 };

            var rooms = await _repository.GetRoomsByBookingsAsync(bookings);

            rooms.Should().HaveCount(1);
            rooms[0].Id.Should().Be(room1.Id);
        }

        [Fact]
        public async Task GetCountRooms_ReturnsCorrectCount()
        {
            for (int i = 1; i <= 5; i++)
            {
                var room = new Room($"Room{i}", 2, "Standard", 100m);
                await _repository.AddRoom(room);
            }
            await _repository.SaveChangesAsync();

            var count = await _repository.GetCountRooms();

            count.Should().Be(5);
        }

        [Fact]
        public async Task GetRoomById_ExistingRoom_ReturnsRoom()
        {
            var room = new Room("Test Room", 2, "Test", 100m);
            await _repository.AddRoom(room);
            await _repository.SaveChangesAsync();

            var result = await _repository.GetRoomById(room.Id);

            result.Should().NotBeNull();
            result!.Title.Should().Be("Test Room");
        }
    }
}