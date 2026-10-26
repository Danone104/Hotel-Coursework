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
    public class RentRepositoryTests : BaseTest
    {
        private readonly RentRepository _repository;

        public RentRepositoryTests()
        {
            _repository = new RentRepository(_context);
        }

        private async Task<(Booking booking, RentRoom rent)> CreateRentedBooking()
        {
            var client = new Client(new PhoneNumber("+79234567890"), new FullName("John", "Doe", null));
            var room = new Room("Test Room", 2, "Test", 100m);
            await _context.Clients.AddAsync(client);
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            var booking = new Booking(client.Id, room.Id,
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 100m);
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            var rent = new RentRoom(booking.Id);
            return (booking, rent);
        }

        [Fact]
        public async Task AddRentAsync_ValidRent_RentAdded()
        {
            var (booking, rent) = await CreateRentedBooking();

            await _repository.AddRentAsync(rent);
            await _repository.SaveChangesAsync();
            var result = await _repository.GetRentByIdAsync(rent.Id);

            result.Should().NotBeNull();
            result!.BookId.Should().Be(booking.Id);
            result.Status.Should().Be(RentStatus.Rented);
        }

        [Fact]
        public async Task GetActiveRentAsync_ReturnsRentedAndChecking()
        {
            var (_, rent1) = await CreateRentedBooking(); 

            var (_, rent2) = await CreateRentedBooking();
            rent2.FixCheckOut(100m); 

            var (_, rent3) = await CreateRentedBooking();
            rent3.FixCheckOut(100m);

            await _repository.AddRentAsync(rent1);
            await _repository.AddRentAsync(rent2);
            await _repository.AddRentAsync(rent3);
            await _repository.SaveChangesAsync();

            var activeRents = await _repository.GetActiveRentAsync();

            activeRents.Should().HaveCount(3);
            activeRents.Should().AllSatisfy(r =>
                r.Status.Should().BeOneOf(RentStatus.Rented, RentStatus.Checking));
        }

        [Fact]
        public async Task GetRentsByBookingsAsync_WithValidBookings_ReturnsCorrectRents()
        {
            var (booking1, rent1) = await CreateRentedBooking();
            var (booking2, rent2) = await CreateRentedBooking();
            var (booking3, rent3) = await CreateRentedBooking();

            await _repository.AddRentAsync(rent1);
            await _repository.AddRentAsync(rent2);
            await _repository.AddRentAsync(rent3);
            await _repository.SaveChangesAsync();

            var bookings = new List<Booking> { booking1, booking2 };

            var rents = await _repository.GetRentsByBookingsAsync(bookings);

            rents.Should().HaveCount(2);
            rents.Select(r => r.BookId).Should().Contain(booking1.Id);
            rents.Select(r => r.BookId).Should().Contain(booking2.Id);
            rents.Select(r => r.BookId).Should().NotContain(booking3.Id);
        }

        [Fact]
        public async Task GetRentsByBookingsAsync_WithEmptyBookingsList_ReturnsEmptyList()
        {
            var (_, rent1) = await CreateRentedBooking();
            await _repository.AddRentAsync(rent1);
            await _repository.SaveChangesAsync();

            var emptyBookings = new List<Booking>();

            var rents = await _repository.GetRentsByBookingsAsync(emptyBookings);

            rents.Should().BeEmpty();
        }

        [Fact]
        public async Task GetRentsByBookingsAsync_WithNoMatchingBookings_ReturnsEmptyList()
        {
            var (booking1, rent1) = await CreateRentedBooking();
            await _repository.AddRentAsync(rent1);
            await _repository.SaveChangesAsync();

            var nonMatchingBooking = new Booking(Guid.NewGuid(), Guid.NewGuid(),
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 100m);

            var bookings = new List<Booking> { nonMatchingBooking };

            var rents = await _repository.GetRentsByBookingsAsync(bookings);

            rents.Should().BeEmpty();
        }

        [Fact]
        public async Task GetRentByIdAsync_ExistingRent_ReturnsRent()
        {
            var (_, rent) = await CreateRentedBooking();
            await _repository.AddRentAsync(rent);
            await _repository.SaveChangesAsync();

            var result = await _repository.GetRentByIdAsync(rent.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(rent.Id);
        }

        [Fact]
        public async Task GetRentByIdAsync_NonExistingRent_ReturnsNull()
        {
            var result = await _repository.GetRentByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Fact]
        public async Task FixCheckOut_CalculatesCorrectPrice()
        {
            var (_, rent) = await CreateRentedBooking();
            await _repository.AddRentAsync(rent);
            await _repository.SaveChangesAsync();

            rent.FixCheckOut(100m);
            await _repository.SaveChangesAsync();
            var result = await _repository.GetRentByIdAsync(rent.Id);

            result.Should().NotBeNull();
            result!.DateCheckOut.Should().NotBeNull();
            result.Status.Should().Be(RentStatus.Checking);
        }

        [Fact]
        public async Task FixCheckOut_SameDayCheckIn_PriceIsHalf()
        {
            var client = new Client(new PhoneNumber("+79234567890"), new FullName("John", "Doe",null));
            var room = new Room("Test Room", 2, "Test", 100m);
            await _context.Clients.AddAsync(client);
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            var booking = new Booking(client.Id, room.Id,
                DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)), 100m);
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            var rent = new RentRoom(booking.Id);
            await _repository.AddRentAsync(rent);
            await _repository.SaveChangesAsync();

            rent.FixCheckOut(100m);
            await _repository.SaveChangesAsync();
            var result = await _repository.GetRentByIdAsync(rent.Id);

            result.Should().NotBeNull();
            result!.Price.Should().Be(50m); 
        }
    }
}