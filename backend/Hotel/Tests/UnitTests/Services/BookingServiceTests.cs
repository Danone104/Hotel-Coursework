using FluentAssertions;
using Hotel.Application.Services;
using Hotel.Domain.Abstractions.Repositories;
using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using Hotel.Domain.ValueObjects;
using Moq;
using Xunit;

namespace Hotel.Application.UnitTests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _bookingRepoMock;
        private readonly Mock<IClientRepository> _clientRepoMock;
        private readonly Mock<IRoomRepository> _roomRepoMock;
        private readonly BookingService _service;

        public BookingServiceTests()
        {
            _bookingRepoMock = new Mock<IBookingRepository>();
            _clientRepoMock = new Mock<IClientRepository>();
            _roomRepoMock = new Mock<IRoomRepository>();

            _service = new BookingService(
                _bookingRepoMock.Object,
                _clientRepoMock.Object,
                _roomRepoMock.Object);
        }

        private Room CreateTestRoom(Guid? roomId = null)
        {
            var room = new Room("Test Room", 2, "Description", 100m);
            typeof(Room).GetProperty("Id")?.SetValue(room, roomId ?? Guid.NewGuid());
            return room;
        }
        private Client CreateTestClient(Guid userId) => new Client(userId, new PhoneNumber("+71234567890"), new FullName("John", "Doe",null));


        [Fact]
        public async Task CreateBookingAsync_RoomNotFound_ThrowsArgumentException()
        {
            _roomRepoMock.Setup(x => x.GetRoomById(It.IsAny<Guid>()))
                .ReturnsAsync((Room?)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CreateBookingAsync(Guid.NewGuid(), Guid.NewGuid(),
                    DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    DateOnly.FromDateTime(DateTime.Now.AddDays(3))));
        }

        [Fact]
        public async Task CreateBookingAsync_ClientNotFound_ThrowsArgumentException()
        {
            var room = CreateTestRoom();
            _roomRepoMock.Setup(x => x.GetRoomById(It.IsAny<Guid>())).ReturnsAsync(room);
            _clientRepoMock.Setup(x => x.GetClientByUserId(It.IsAny<Guid>()))
                .ReturnsAsync((Client?)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CreateBookingAsync(Guid.NewGuid(), Guid.NewGuid(),
                    DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    DateOnly.FromDateTime(DateTime.Now.AddDays(3))));
        }

        [Fact]
        public async Task CreateBookingAsync_RoomNotAvailable_ThrowsArgumentException()
        {
            var room = CreateTestRoom();
            var client = CreateTestClient(Guid.NewGuid());
            var existingBooking = new Booking(client.Id, room.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 100m);

            _roomRepoMock.Setup(x => x.GetRoomById(It.IsAny<Guid>())).ReturnsAsync(room);
            _clientRepoMock.Setup(x => x.GetClientByUserId(It.IsAny<Guid>())).ReturnsAsync(client);
            _bookingRepoMock.Setup(x => x.GetOverlappingWithPeriodAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<Booking> { existingBooking });

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CreateBookingAsync(Guid.NewGuid(), Guid.NewGuid(),
                    DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    DateOnly.FromDateTime(DateTime.Now.AddDays(3))));
        }

        [Fact]
        public async Task CancelBookAsync_ValidBooking_CancelsBooking()
        {
            var bookingId = Guid.NewGuid();
            var reason = "Test reason";
            var booking = new Booking(Guid.NewGuid(), Guid.NewGuid(),
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 100m);

            _bookingRepoMock.Setup(x => x.GetBookingByIdAsync(bookingId))
                .ReturnsAsync(booking);

            await _service.CancelBookAsync(bookingId, reason);

            booking.Status.Should().Be(BookStatus.Canceled);
            booking.ReasonCancel.Should().Be(reason);
            _bookingRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CancelBookAsync_BookingNotFound_ThrowsArgumentException()
        {
            _bookingRepoMock.Setup(x => x.GetBookingByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Booking?)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CancelBookAsync(Guid.NewGuid(), "reason"));
        }

        

        [Fact]
        public async Task GetOccupiedDatesByMonthAsync_ReturnsOccupiedDates()
        {
            var roomId = Guid.NewGuid();
            var year = 2026;
            var month = 4;
            var room = CreateTestRoom(roomId);
            var checkIn = DateOnly.FromDateTime(DateTime.UtcNow);
            var checkOut = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(4);
            var booking = new Booking(Guid.NewGuid(), roomId, checkIn, checkOut, 100m);

            _roomRepoMock.Setup(x => x.GetRoomById(roomId)).ReturnsAsync(room);
            _bookingRepoMock.Setup(x => x.GetOverlappingWithPeriodAsync(roomId, It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<Booking> { booking });

            var result = await _service.GetOccupiedDatesByMonthAsync(roomId, year, month);

            result.Should().HaveCount(5); 
        }

        [Fact]
        public async Task GetBookingByIdAsync_ExistingBooking_ReturnsBooking()
        {
            var bookingId = Guid.NewGuid();
            var expectedBooking = new Booking(Guid.NewGuid(), Guid.NewGuid(),
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 100m);

            _bookingRepoMock.Setup(x => x.GetBookingByIdAsync(bookingId))
                .ReturnsAsync(expectedBooking);

            var result = await _service.GetBookingByIdAsync(bookingId);

            result.Should().Be(expectedBooking);
        }

        [Fact]
        public async Task GetBookingByIdAsync_NonExistingBooking_ThrowsArgumentException()
        {
            _bookingRepoMock.Setup(x => x.GetBookingByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Booking?)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.GetBookingByIdAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetActiveBookingsAsync_ReturnsActiveBookings()
        {
            var expectedBookings = new List<Booking>();
            _bookingRepoMock.Setup(x => x.GetActiveBookingAsync())
                .ReturnsAsync(expectedBookings);

            var result = await _service.GetActiveBookingsAsync();

            result.Should().BeSameAs(expectedBookings);
        }
    }
}