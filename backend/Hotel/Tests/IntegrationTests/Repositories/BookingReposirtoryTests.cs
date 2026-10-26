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
    public class BookingRepositoryTests : BaseTest
    {
        private readonly BookingRepository _repository;

        public BookingRepositoryTests()
        {
            _repository = new BookingRepository(_context);
        }

        private async Task<(Client client, Room room)> CreateTestClientAndRoom()
        {
            var client = new Client(
                new PhoneNumber("+71234567890"),
                new FullName("John", "Doe",null));

            var room = new Room("Test Room", 2, "Test description", 100m);

            await _context.Clients.AddAsync(client);
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            return (client, room);
        }

        private async Task<Booking> CreateTestBooking()
        {
            var (client, room) = await CreateTestClientAndRoom();

            var booking = new Booking(
                client.Id,
                room.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                room.Price);

            return booking;
        }

        [Fact]
        public async Task AddBook_ValidBooking_BookingAdded()
        {
            var booking = await CreateTestBooking();

            await _repository.AddBook(booking);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var result = await _repository.GetBookingByIdAsync(booking.Id);

            result.Should().NotBeNull();
            result!.Status.Should().Be(BookStatus.Book);
            result.ClientId.Should().Be(booking.ClientId);
            result.RoomId.Should().Be(booking.RoomId);
        }

        [Fact]
        public async Task GetActiveBookingAsync_ReturnsOnlyActiveBookings()
        {
            var (client1, room1) = await CreateTestClientAndRoom();
            var (client2, room2) = await CreateTestClientAndRoom();

            var activeBooking = new Booking(
                client1.Id, room1.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                100m);

            var cancelledBooking = new Booking(
                client2.Id, room2.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(4)),
                100m);
            cancelledBooking.CancelBookByClient();

            await _repository.AddBook(activeBooking);
            await _repository.AddBook(cancelledBooking);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var active = await _repository.GetActiveBookingAsync();

            active.Should().HaveCount(1);
            active[0].Status.Should().Be(BookStatus.Book);
        }

        [Fact]
        public async Task GetRentedBookingsAsync_ReturnsRentedBookings()
        {
            var (client1, room1) = await CreateTestClientAndRoom();
            var (client2, room2) = await CreateTestClientAndRoom();

            var booked = new Booking(
                client1.Id, room1.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                100m);

            var rented = new Booking(
                client2.Id, room2.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(4)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(6)),
                100m);
            rented.BookToRent();

            await _repository.AddBook(booked);
            await _repository.AddBook(rented);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var rentedBookings = await _repository.GetRentedBookingsAsync();

            rentedBookings.Should().HaveCount(1);
            rentedBookings[0].Status.Should().Be(BookStatus.Rented);
        }

        [Fact]
        public async Task GetOverlappingWithPeriodAsync_ReturnsOverlappingBookings()
        {
            // Arrange
            var (client, room) = await CreateTestClientAndRoom();

            var existingBooking = new Booking(
                client.Id, room.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
                100m);

            await _repository.AddBook(existingBooking);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var overlapping = await _repository.GetOverlappingWithPeriodAsync(room.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(12)));

            overlapping.Should().HaveCount(1);
            overlapping[0].Id.Should().Be(existingBooking.Id);
        }

        [Fact]
        public async Task GetOverlappingWithPeriodAsync_NoOverlap_ReturnsEmpty()
        {
            var (client, room) = await CreateTestClientAndRoom();

            var existingBooking = new Booking(
                client.Id, room.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
                100m);

            await _repository.AddBook(existingBooking);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var overlapping = await _repository.GetOverlappingWithPeriodAsync(room.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(12)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(15)));

            overlapping.Should().BeEmpty();
        }

        [Fact]
        public async Task GetActiveBookingsByRoomAsync_ReturnsCorrectBookings()
        {
            var (client, room) = await CreateTestClientAndRoom();
            var (_, otherRoom) = await CreateTestClientAndRoom();

            var booking1 = new Booking(
                client.Id, room.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                100m);

            var booking2 = new Booking(
                client.Id, room.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(4)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(6)),
                100m);

            var booking3 = new Booking(
                client.Id, otherRoom.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                100m);

            await _repository.AddBook(booking1);
            await _repository.AddBook(booking2);
            await _repository.AddBook(booking3);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var roomBookings = await _repository.GetActiveBookingsByRoomAsync(room.Id);

            roomBookings.Should().HaveCount(2);
            roomBookings.Should().AllSatisfy(b => b.RoomId.Should().Be(room.Id));
        }

        [Fact]
        public async Task GetActiveByClientByPageAsync_ReturnsPaginatedResults()
        {
            var (client, room) = await CreateTestClientAndRoom();

            for (int i = 1; i <= 10; i++)
            {
                var booking = new Booking(
                    client.Id, room.Id,
                    DateOnly.FromDateTime(DateTime.Now.AddDays(i)),
                    DateOnly.FromDateTime(DateTime.Now.AddDays(i + 2)),
                    100m);
                await _repository.AddBook(booking);
            }
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var page1 = await _repository.GetActiveByClientByPageAsync(client.Id, 1, 3);
            var page2 = await _repository.GetActiveByClientByPageAsync(client.Id, 2, 3);

            page1.Should().HaveCount(3);
            page2.Should().HaveCount(3);
            page1.Should().AllSatisfy(b => b.ClientId.Should().Be(client.Id));
            page1.Select(b => b.Id).Should().NotIntersectWith(page2.Select(b => b.Id));
        }

        [Fact]
        public async Task GetTodaysActiveBookingAsync_ReturnsBookingsForToday()
        {
            var (client, room) = await CreateTestClientAndRoom();
            var (_, otherRoom) = await CreateTestClientAndRoom();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            var todayBooking = new Booking(
                client.Id, room.Id,
                today,
                today.AddDays(2),
                100m);

            var tomorrowBooking = new Booking(
                client.Id, otherRoom.Id,
                tomorrow,
                tomorrow.AddDays(2),
                100m);

            await _repository.AddBook(todayBooking);
            await _repository.AddBook(tomorrowBooking);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var todaysBookings = await _repository.GetTodaysActiveBookingAsync();

            todaysBookings.Should().HaveCount(1);
            todaysBookings[0].DateCheckIn.Should().Be(today);
            todaysBookings[0].Id.Should().Be(todayBooking.Id);
        }

        [Fact]
        public async Task GetActiveAndCancelledForDateAsync_ReturnsCorrectStatuses()
        {
            var (client, room) = await CreateTestClientAndRoom();
            var (_, otherRoom) = await CreateTestClientAndRoom();
            var (_, thirdRoom) = await CreateTestClientAndRoom();

            var today = DateOnly.FromDateTime(DateTime.Now);

            var active = new Booking(
                client.Id, room.Id,
                today, today.AddDays(2),
                100m);

            var cancelled = new Booking(
                client.Id, otherRoom.Id,
                today, today.AddDays(2),
                100m);
            cancelled.CancelBookByClient();

            var ended = new Booking(
                client.Id, thirdRoom.Id,
                today, today.AddDays(2),
                100m);
            ended.EndedBook();

            await _repository.AddBook(active);
            await _repository.AddBook(cancelled);
            await _repository.AddBook(ended);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var result = await _repository.GetActiveAndCancelledForDateAsync(today);

            result.Should().HaveCount(2);
            result.Should().NotContain(b => b.Status == BookStatus.Ended);
            result.Should().Contain(b => b.Status == BookStatus.Book);
            result.Should().Contain(b => b.Status == BookStatus.Canceled);
        }

        [Fact]
        public async Task GetBookingByIdAsync_ExistingBooking_ReturnsBooking()
        {
            var booking = await CreateTestBooking();
            await _repository.AddBook(booking);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var result = await _repository.GetBookingByIdAsync(booking.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(booking.Id);
        }

        [Fact]
        public async Task CancelBook_ActiveBooking_ChangesStatusToCanceled()
        {
            var booking = await CreateTestBooking();
            await _repository.AddBook(booking);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var bookingToCancel = await _repository.GetBookingByIdAsync(booking.Id);

            bookingToCancel!.CancelBook("Test reason");
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var result = await _repository.GetBookingByIdAsync(booking.Id);

            result.Should().NotBeNull();
            result!.Status.Should().Be(BookStatus.Canceled);
            result.ReasonCancel.Should().Be("Test reason");
        }
        [Fact]
        public async Task ExtendBooking_UpdatesDatesAndPrice()
        {
            var booking = await CreateTestBooking();
            await _repository.AddBook(booking);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var bookingToUpdate = await _repository.GetBookingByIdAsync(booking.Id);

            var pricePerNight = booking.Price / (booking.DateCheckOut.DayNumber - booking.DateCheckIn.DayNumber);

            var newCheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(5));

            var newPrice = (newCheckOut.DayNumber - booking.DateCheckIn.DayNumber) * pricePerNight;

            typeof(Booking).GetProperty("DateCheckOut")?.SetValue(bookingToUpdate, newCheckOut);
            typeof(Booking).GetProperty("Price")?.SetValue(bookingToUpdate, newPrice);

            _context.Entry(bookingToUpdate).Property(x => x.DateCheckOut).IsModified = true;
            _context.Entry(bookingToUpdate).Property(x => x.Price).IsModified = true;
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var updated = await _repository.GetBookingByIdAsync(booking.Id);

            // Assert
            updated.Should().NotBeNull();
            updated!.DateCheckOut.Should().Be(newCheckOut);

            var expectedDays = newCheckOut.DayNumber - booking.DateCheckIn.DayNumber;
            var expectedPrice = expectedDays * pricePerNight;
            updated.Price.Should().Be(expectedPrice);
        }
    }
}