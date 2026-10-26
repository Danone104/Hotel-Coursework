using FluentAssertions;
using Hotel.Application.Services;
using Hotel.Domain.Abstractions.Repositories;
using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using Moq;
using Xunit;

namespace Hotel.Application.UnitTests.Services
{
    public class RoomServiceTests
    {
        private readonly Mock<IRoomRepository> _roomRepoMock;
        private readonly RoomService _service;

        public RoomServiceTests()
        {
            _roomRepoMock = new Mock<IRoomRepository>();
            _service = new RoomService(_roomRepoMock.Object);
        }

        [Fact]
        public async Task CreateRoom_ValidData_CreatesAndReturnsRoomId()
        {
            var title = "Suite";
            var count = 4;
            var description = "Luxury suite";
            var price = 250m;

            var result = await _service.CreateRoom(title, count, description, price);

            result.Should().NotBeEmpty();
            _roomRepoMock.Verify(x => x.AddRoom(It.Is<Room>(r =>
                r.Title == title && r.CountPerson == count && r.Price == price)), Times.Once);
            _roomRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetRoomById_ExistingRoom_ReturnsRoom()
        {
            var roomId = Guid.NewGuid();
            var expectedRoom = new Room("Test", 2, "Desc", 100m);
            _roomRepoMock.Setup(x => x.GetRoomById(roomId)).ReturnsAsync(expectedRoom);

            var result = await _service.GetRoomById(roomId);

            result.Should().Be(expectedRoom);
        }

        [Fact]
        public async Task GetRoomById_NonExistingRoom_ThrowsArgumentException()
        {
            _roomRepoMock.Setup(x => x.GetRoomById(It.IsAny<Guid>()))
                .ReturnsAsync((Room?)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.GetRoomById(Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateRoom_ValidData_UpdatesRoom()
        {
            var roomId = Guid.NewGuid();
            var room = new Room("Old Title", 2, "Old Desc", 100m);
            _roomRepoMock.Setup(x => x.GetRoomById(roomId)).ReturnsAsync(room);

            await _service.UpdateRoom(roomId, "New Title", "New Desc", 150m, RoomStatus.Ready, 3);

            room.Title.Should().Be("New Title");
            room.Description.Should().Be("New Desc");
            room.Price.Should().Be(150m);
            room.CountPerson.Should().Be(3);
            _roomRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteRoom_ExistingRoom_DeletesRoom()
        {
            var roomId = Guid.NewGuid();
            var room = new Room("Test", 2, "Desc", 100m);
            _roomRepoMock.Setup(x => x.GetRoomById(roomId)).ReturnsAsync(room);

            await _service.DeleteRoom(roomId);

            room.Status.Should().Be(RoomStatus.Deleted);
            _roomRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetRoomsAsync_ReturnsAllRooms()
        {
            var expectedRooms = new List<Room> { new Room("Room1", 2, "Desc", 100m) };
            _roomRepoMock.Setup(x => x.GetRooms()).ReturnsAsync(expectedRooms);

            var result = await _service.GetRoomsAsync();

            result.Should().BeSameAs(expectedRooms);
        }

        [Fact]
        public async Task GetRoomsByPageAsync_ReturnsPaginatedRooms()
        {
            var expectedRooms = new List<Room>();
            _roomRepoMock.Setup(x => x.GetRoomsByPage(1, 10)).ReturnsAsync(expectedRooms);

            var result = await _service.GetRoomsByPageAsync(1, 10);

            result.Should().BeSameAs(expectedRooms);
        }
    }
}