using FluentAssertions;
using Hotel.Application.Services;
using Hotel.Domain.Abstractions.Repositories;
using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using Moq;
using Xunit;

namespace Hotel.Application.UnitTests.Services
{
    public class RentServiceTests
    {
        private readonly Mock<IBookingRepository> _bookRepoMock;
        private readonly Mock<IRentRepository> _rentRepoMock;
        private readonly Mock<IClientRepository> _clientRepoMock;
        private readonly Mock<IRoomRepository> _roomRepoMock;
        private readonly RentService _service;

        public RentServiceTests()
        {
            _bookRepoMock = new Mock<IBookingRepository>();
            _rentRepoMock = new Mock<IRentRepository>();
            _clientRepoMock = new Mock<IClientRepository>();
            _roomRepoMock = new Mock<IRoomRepository>();

            _service = new RentService(
                _bookRepoMock.Object,
                _rentRepoMock.Object,
                _clientRepoMock.Object,
                _roomRepoMock.Object);
        }

        [Fact]
        public async Task RentRoom_ValidBooking_CreatesRent()
        {
            var bookId = Guid.NewGuid();
            var booking = new Booking(Guid.NewGuid(), Guid.NewGuid(),
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 100m);

            _bookRepoMock.Setup(x => x.GetBookingByIdAsync(bookId))
                .ReturnsAsync(booking);

            await _service.RentRoom(bookId);

            booking.Status.Should().Be(BookStatus.Rented);
            _rentRepoMock.Verify(x => x.AddRentAsync(It.Is<RentRoom>(r =>
                r.BookId == bookId)), Times.Once);
            _rentRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RentRoom_BookingNotFound_ThrowsArgumentException()
        {
            _bookRepoMock.Setup(x => x.GetBookingByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Booking?)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.RentRoom(Guid.NewGuid()));
        }

        [Fact]
        public async Task FixCheckout_ValidRent_FixesCheckout()
        {
            var rentId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var roomId = Guid.NewGuid();
            var rent = new RentRoom(bookId);
            var booking = new Booking(Guid.NewGuid(), roomId,
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 100m);
            var room = new Room("Test", 2, "Desc", 100m);

            _rentRepoMock.Setup(x => x.GetRentByIdAsync(rentId))
                .ReturnsAsync(rent);
            _bookRepoMock.Setup(x => x.GetBookingByIdAsync(bookId))
                .ReturnsAsync(booking);
            _roomRepoMock.Setup(x => x.GetRoomById(roomId))
                .ReturnsAsync(room);

            await _service.FixCheckout(rentId);

            rent.Status.Should().Be(RentStatus.Checking);
            _rentRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddNoteAsync_ValidRent_AddsNote()
        {
            var rentId = Guid.NewGuid();
            var rent = new RentRoom(Guid.NewGuid());
            var note = "Test note";

            _rentRepoMock.Setup(x => x.GetRentByIdAsync(rentId))
                .ReturnsAsync(rent);

            await _service.AddNoteAsync(rentId, note);

            rent.Note.Should().Be(note);
        }

        [Fact]
        public async Task GetActiveRentsAsync_ReturnsActiveRents()
        {
            var expectedRents = new List<RentRoom>();
            _rentRepoMock.Setup(x => x.GetActiveRentAsync())
                .ReturnsAsync(expectedRents);

            var result = await _service.GetActiveRentsAsync();

            result.Should().BeSameAs(expectedRents);
        }
    }
}