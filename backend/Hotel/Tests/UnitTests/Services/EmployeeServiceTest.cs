using FluentAssertions;
using Hotel.Application.Services;
using Hotel.Domain.Abstractions.Repositories;
using Hotel.Domain.Abstractions.Security;
using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using Hotel.Domain.ValueObjects;
using Moq;
using Xunit;

namespace Hotel.Application.UnitTests.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            _employeeRepoMock = new Mock<IEmployeeRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();

            _service = new EmployeeService(
                _employeeRepoMock.Object,
                _userRepoMock.Object,
                _passwordHasherMock.Object);
        }

        [Fact]
        public async Task CreateEmployee_ValidData_CreatesEmployee()
        {
            var userName = "empuser";
            var password = "password123";
            var firstName = "John";
            var lastName = "Doe";
            string? patronymic = null;
            var phone = "+71234567890";
            var role = Roles.Manager;
            var passwordHash = "hashedPass";

            _userRepoMock.Setup(x => x.GetUserByUserName(userName))
                .ReturnsAsync((User?)null);
            _passwordHasherMock.Setup(x => x.Hash(password))
                .ReturnsAsync(passwordHash);
            _userRepoMock.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            await _service.CreateEmployee(userName, password, firstName, lastName, patronymic, phone, role);

            _userRepoMock.Verify(x => x.AddUser(It.Is<User>(u =>
                u.UserName == userName && u.Role == role)), Times.Once);

            _employeeRepoMock.Verify(x => x.AddEmployeeAsync(It.Is<Employee>(e =>
                e.Name.FirstName == firstName && e.Name.LastName == lastName)), Times.Once);
        }

        [Fact]
        public async Task CreateEmployee_UserNameTaken_ThrowsArgumentException()
        {
            var existingUser = new User("hash", "taken", Roles.Client);
            _userRepoMock.Setup(x => x.GetUserByUserName("taken"))
                .ReturnsAsync(existingUser);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CreateEmployee("taken", "pass", "John", "Doe", null, "+71234567890", Roles.Manager));
        }

        [Fact]
        public async Task DeleteEmployee_ValidEmployee_DeletesEmployeeAndUser()
        {
            var employeeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var user = new User("hash", "user", Roles.Manager);
            var employee = new Employee(userId, new FullName("John", "Doe",null), new PhoneNumber("+71234567890"));

            _employeeRepoMock.Setup(x => x.GetEmployeeByIdAsync(employeeId))
                .ReturnsAsync(employee);
            _userRepoMock.Setup(x => x.GetUserById(userId))
                .ReturnsAsync(user);

            await _service.DeleteEmployee(employeeId);

            _userRepoMock.Verify(x => x.DeleteUserAsync(user), Times.Once);
            _userRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetEmployeesAsync_ReturnsAllEmployees()
        {
            var expectedEmployees = new List<Employee>();
            _employeeRepoMock.Setup(x => x.GetEmployeesAsync())
                .ReturnsAsync(expectedEmployees);

            var result = await _service.GetEmployeesAsync();

            result.Should().BeSameAs(expectedEmployees);
        }

        [Fact]
        public async Task GetUsersEmployeesAsync_ReturnsEmployeeUsers()
        {
            var expectedUsers = new List<User>();
            _userRepoMock.Setup(x => x.GetUsersEmployeesAsync())
                .ReturnsAsync(expectedUsers);

            var result = await _service.GetUsersEmployeesAsync();

            result.Should().BeSameAs(expectedUsers);
        }
    }
}