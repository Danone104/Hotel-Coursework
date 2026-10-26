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
    public class RentHistoryServiceTests
    {
        private readonly Mock<IRentRepository> _rentRepoMock;
        private readonly Mock<IClientRepository> _clientRepoMock;
        private readonly Mock<IBookingRepository> _bookingRepoMock;
        private readonly RentHistoryService _service;

        public RentHistoryServiceTests()
        {
            _rentRepoMock = new Mock<IRentRepository>();
            _clientRepoMock = new Mock<IClientRepository>();
            _bookingRepoMock = new Mock<IBookingRepository>();

            _service = new RentHistoryService(
                _rentRepoMock.Object,
                _clientRepoMock.Object,
                _bookingRepoMock.Object);
        }

        private Client CreateTestClient(Guid userId) => new Client(userId, new PhoneNumber("+71234567890"), new FullName("John", "Doe",null));

        [Fact]
        public async Task GetRentsHistoryAsync_ValidUser_ReturnsRents()
        {
            var userId = Guid.NewGuid();
            var client = CreateTestClient(userId);
            var bookings = new List<Booking>();
            var rents = new List<RentRoom>();

            _clientRepoMock.Setup(x => x.GetClientByUserId(userId))
                .ReturnsAsync(client);
            _bookingRepoMock.Setup(x => x.GetForRentHistorytByPageAsync(client.Id, 1, 10))
                .ReturnsAsync(bookings);
            _rentRepoMock.Setup(x => x.GetRentsByBookingsAsync(bookings))
                .ReturnsAsync(rents);

            var result = await _service.GetRentsHistoryAsync(userId, 1, 10);

            result.Should().BeEmpty(); 
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetRentsHistoryAsync_ClientNotFound_ThrowsArgumentException()
        {
            _clientRepoMock.Setup(x => x.GetClientByUserId(It.IsAny<Guid>()))
                .ReturnsAsync((Client?)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.GetRentsHistoryAsync(Guid.NewGuid(), 1, 10));
        }

        [Fact]
        public async Task GetActiveBookingsAsync_ValidUser_ReturnsBookings()
        {
            var userId = Guid.NewGuid();
            var client = CreateTestClient(userId);
            var expectedBookings = new List<Booking>();

            _clientRepoMock.Setup(x => x.GetClientByUserId(userId))
                .ReturnsAsync(client);
            _bookingRepoMock.Setup(x => x.GetActiveByClientByPageAsync(client.Id, 1, 10))
                .ReturnsAsync(expectedBookings);

            var result = await _service.GetActiveBookingsAsync(userId, 1, 10);

            result.Should().BeSameAs(expectedBookings);
        }

        [Fact]
        public async Task CancelActiveBooking_ValidRequest_CancelsBooking()
        {
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var client = CreateTestClient(userId);
            var booking = new Booking(client.Id, Guid.NewGuid(),
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 100m);

            _clientRepoMock.Setup(x => x.GetClientByUserId(userId))
                .ReturnsAsync(client);
            _bookingRepoMock.Setup(x => x.GetBookingByIdAsync(bookId))
                .ReturnsAsync(booking);

            await _service.CancelActiveBooking(userId, bookId);

            booking.Status.Should().Be(BookStatus.Canceled);
            booking.ReasonCancel.Should().Be("Отменено клиентом");
            _bookingRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CancelActiveBooking_WrongClient_ThrowsArgumentException()
        {
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var client = CreateTestClient(userId);
            var booking = new Booking(Guid.NewGuid(), Guid.NewGuid(),
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 100m);

            _clientRepoMock.Setup(x => x.GetClientByUserId(userId))
                .ReturnsAsync(client);
            _bookingRepoMock.Setup(x => x.GetBookingByIdAsync(bookId))
                .ReturnsAsync(booking);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CancelActiveBooking(userId, bookId));
        }
    }
}