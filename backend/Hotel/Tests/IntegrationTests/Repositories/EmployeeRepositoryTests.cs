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
    public class EmployeeRepositoryTests : BaseTest
    {
        private readonly EmployeeRepository _repository;

        public EmployeeRepositoryTests()
        {
            _repository = new EmployeeRepository(_context);
        }

        // Вспомогательный метод для создания пользователя
        private async Task<User> CreateTestUser(string userName = "TestUser")
        {
            var user = new User("hashedPassword", userName, Roles.Admin);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return user;
        }

        // Вспомогательный метод для создания Employee
        private async Task<Employee> CreateTestEmployee(Guid? userId = null)
        {
            if (userId == null)
            {
                var user = await CreateTestUser($"User_{Guid.NewGuid()}");
                userId = user.Id;
            }

            var name = new FullName("John", "Doe",null);
            var phone = new PhoneNumber("+79234567890");
            var employee = new Employee(userId.Value, name, phone);

            return employee;
        }

        [Fact]
        public async Task AddEmployeeAsync_ValidEmployee_EmployeeAdded()
        {
            // Arrange
            var user = await CreateTestUser("EmployeeUser");
            var name = new FullName("John", "Doe",null);
            var phone = new PhoneNumber("+79234567890");
            var employee = new Employee(user.Id, name, phone);

            // Act
            await _repository.AddEmployeeAsync(employee);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var result = await _repository.GetEmployeeByIdAsync(employee.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Name.FirstName.Should().Be("John");
            result.Name.LastName.Should().Be("Doe");
            result.TelephoneNum.Value.Should().Be("+79234567890");
            result.UserId.Should().Be(user.Id);
        }

        [Fact]
        public async Task GetEmployeesAsync_ReturnsAllEmployees()
        {
            // Arrange
            var user1 = await CreateTestUser("Emp1");
            var user2 = await CreateTestUser("Emp2");

            var emp1 = new Employee(user1.Id, new FullName("John", "Doe", null), new PhoneNumber("+79234567890"));
            var emp2 = new Employee(user2.Id, new FullName("Jane", "Smith",null), new PhoneNumber("+79234567891"));

            await _repository.AddEmployeeAsync(emp1);
            await _repository.AddEmployeeAsync(emp2);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            // Act
            var employees = await _repository.GetEmployeesAsync();

            // Assert
            employees.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ExistingEmployee_ReturnsEmployee()
        {
            // Arrange
            var employee = await CreateTestEmployee();
            await _repository.AddEmployeeAsync(employee);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            // Act
            var result = await _repository.GetEmployeeByIdAsync(employee.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(employee.Id);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_NonExistingEmployee_ReturnsNull()
        {
            // Act
            var result = await _repository.GetEmployeeByIdAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetEmployeesByNameAsync_ReturnsMatchingEmployees()
        {
            // Arrange
            var user1 = await CreateTestUser("User1");
            var user2 = await CreateTestUser("User2");
            var user3 = await CreateTestUser("User3");

            var name = new FullName("John", "Doe", null);
            var otherName = new FullName("Jane", "Smith", null);

            var emp1 = new Employee(user1.Id, name, new PhoneNumber("+79234567890"));
            var emp2 = new Employee(user2.Id, name, new PhoneNumber("+79234567891"));
            var emp3 = new Employee(user3.Id, otherName, new PhoneNumber("+79234567892"));

            await _repository.AddEmployeeAsync(emp1);
            await _repository.AddEmployeeAsync(emp2);
            await _repository.AddEmployeeAsync(emp3);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            // Act
            var result = await _repository.GetEmployeesByNameAsync(name);

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(e => e.Name.Should().Be(name));
        }

        [Fact]
        public async Task GetEmployeeByUserIdAsync_ExistingUser_ReturnsEmployee()
        {
            // Arrange
            var user = await CreateTestUser("EmployeeUser");
            var employee = new Employee(user.Id, new FullName("John", "Doe", null), new PhoneNumber("+79234567890"));

            await _repository.AddEmployeeAsync(employee);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            // Act
            var result = await _repository.GetEmployeeByUserIdAsync(user.Id);

            // Assert
            result.Should().NotBeNull();
            result!.UserId.Should().Be(user.Id);
        }

        [Fact]
        public async Task GetEmployeeByUserIdAsync_NonExistingUser_ReturnsNull()
        {
            // Arrange
            var user = await CreateTestUser("SomeUser");
            var employee = new Employee(user.Id, new FullName("John", "Doe", null), new PhoneNumber("+79234567890"));

            await _repository.AddEmployeeAsync(employee);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            // Act
            var result = await _repository.GetEmployeeByUserIdAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateEmployeeAsync_EmployeeUpdated_Successfully()
        {
            // Arrange
            var user = await CreateTestUser("UpdateUser");
            var employee = new Employee(user.Id, new FullName("Old", "Name", null), new PhoneNumber("+79234567890"));

            await _repository.AddEmployeeAsync(employee);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            // Act
            var employeeToUpdate = await _repository.GetEmployeeByIdAsync(employee.Id);
            var newName = new FullName("New", "Name", null);
            var newPhone = new PhoneNumber("+79234567899");

            employeeToUpdate!.UpdateEmployeeInfo(newName, newPhone);

            _context.Entry(employeeToUpdate).State = EntityState.Modified;
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var result = await _repository.GetEmployeeByIdAsync(employee.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Name.FirstName.Should().Be("New");
            result.Name.LastName.Should().Be("Name");
            result.TelephoneNum.Value.Should().Be("+79234567899");
        }

        [Fact]
        public async Task DeleteEmployeeAsync_EmployeeExists_EmployeeRemoved()
        {
            // Arrange
            var user = await CreateTestUser("DeleteUser");
            var employee = new Employee(user.Id, new FullName("ToDelete", "Name",null), new PhoneNumber("+79234567890"));

            await _repository.AddEmployeeAsync(employee);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            // Act
            var employeeToDelete = await _repository.GetEmployeeByIdAsync(employee.Id);
            await _repository.DeleteEmployeeAsync(employeeToDelete!);
            await _repository.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var result = await _repository.GetEmployeeByIdAsync(employee.Id);

            // Assert
            result.Should().BeNull();
        }
    }
}