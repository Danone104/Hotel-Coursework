using FluentAssertions;
using Hotel.DataAccess.Repositories;
using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Tests.IntegrationTests;
using Xunit;

namespace Hotel.IntegrationTests.Repositories
{
    public class UserRepositoryTests : BaseTest
    {
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            _repository = new UserRepository(_context);
        }

        [Fact]
        public async Task AddUser_ValidUser_UserAdded()
        {
            var user = new User("hashedPass123", "JohnDoe", Roles.Admin);

            await _repository.AddUser(user);
            await _repository.SaveChangesAsync();
            var result = await _repository.GetUserById(user.Id);

            result.Should().NotBeNull();
            result!.UserName.Should().Be("JohnDoe");
            result.Role.Should().Be(Roles.Admin);
        }

        [Fact]
        public async Task IsSystemHaveAdminsAsync_WithAdmin_ReturnsTrue()
        {
            var admin = new User("hash", "Admin1", Roles.Admin);
            var client = new User("hash", "Client1", Roles.Client);
            await _repository.AddUser(admin);
            await _repository.AddUser(client);
            await _repository.SaveChangesAsync();

            var result = await _repository.IsSystemHaveAdminsAsync();

            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsSystemHaveAdminsAsync_WithoutAdmin_ReturnsFalse()
        {
            var client = new User("hash", "Client1", Roles.Client);
            await _repository.AddUser(client);
            await _repository.SaveChangesAsync();

            var result = await _repository.IsSystemHaveAdminsAsync();

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetUsersEmployeesAsync_ReturnsOnlyNonClients()
        {
            var admin = new User("hash", "Admin1", Roles.Admin);
            var manager = new User("hash", "Manager1", Roles.Manager);
            var client = new User("hash", "Client1", Roles.Client);

            await _repository.AddUser(admin);
            await _repository.AddUser(manager);
            await _repository.AddUser(client);
            await _repository.SaveChangesAsync();

            var employees = await _repository.GetUsersEmployeesAsync();

            employees.Should().HaveCount(2);
            employees.Should().AllSatisfy(e => e.Role.Should().NotBe(Roles.Client));
        }

        [Fact]
        public async Task GetUserByUserName_ExistingUser_ReturnsUser()
        {
            var user = new User("hash", "UniqueUser123", Roles.Client);
            await _repository.AddUser(user);
            await _repository.SaveChangesAsync();

            var result = await _repository.GetUserByUserName("UniqueUser123");

            result.Should().NotBeNull();
            result!.UserName.Should().Be("UniqueUser123");
        }

        [Fact]
        public async Task GetUserByUserName_NonExistingUser_ReturnsNull()
        {
            var result = await _repository.GetUserByUserName("NonExistent");

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUserById_ExistingUser_ReturnsUser()
        {
            var user = new User("hash", "TestUser", Roles.Client);
            await _repository.AddUser(user);
            await _repository.SaveChangesAsync();

            var result = await _repository.GetUserById(user.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task DeleteUserAsync_UserExists_UserRemoved()
        {
            var user = new User("hash", "ToDelete", Roles.Client);
            await _repository.AddUser(user);
            await _repository.SaveChangesAsync();

            await _repository.DeleteUserAsync(user);
            await _repository.SaveChangesAsync();
            var result = await _repository.GetUserById(user.Id);

            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateUserAsync_UserUpdated_Successfully()
        {
            var user = new User("oldHash", "OldName", Roles.Client);
            await _repository.AddUser(user);
            await _repository.SaveChangesAsync();

            user.UpdateUserName("NewName");
            await _repository.UpdateUserAsync(user);
            await _repository.SaveChangesAsync();
            var result = await _repository.GetUserById(user.Id);

            result.Should().NotBeNull();
            result!.UserName.Should().Be("NewName");
        }
    }
}