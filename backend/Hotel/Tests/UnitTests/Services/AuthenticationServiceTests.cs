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
    public class AuthenticationServiceTests
    {
        private readonly Mock<IClientRepository> _clientRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IJwtProvider> _jwtProviderMock;
        private readonly AuthenticationService _service;

        public AuthenticationServiceTests()
        {
            _clientRepoMock = new Mock<IClientRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _jwtProviderMock = new Mock<IJwtProvider>();

            _service = new AuthenticationService(
                _clientRepoMock.Object,
                _userRepoMock.Object,
                _passwordHasherMock.Object,
                _jwtProviderMock.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            var userName = "testuser";
            var password = "password123";
            var user = new User("hashedPass", userName, Roles.Client);
            var expectedToken = "jwt_token_123";

            _userRepoMock.Setup(x => x.GetUserByUserName(userName))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyPassword(password, user.PasswordHash))
                .ReturnsAsync(true);
            _jwtProviderMock.Setup(x => x.GenerateToken(user))
                .Returns(expectedToken);

            var result = await _service.Login(userName, password);

            result.Should().Be(expectedToken);
        }

        [Fact]
        public async Task Login_EmptyUserName_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.Login("", "password"));
        }

        [Fact]
        public async Task Login_EmptyPassword_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.Login("username", ""));
        }

        [Fact]
        public async Task Login_UserNotFound_ThrowsUnauthorizedAccessException()
        {
            _userRepoMock.Setup(x => x.GetUserByUserName("unknown"))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.Login("unknown", "password"));
        }

        [Fact]
        public async Task Login_WrongPassword_ThrowsUnauthorizedAccessException()
        {
            var user = new User("hashedPass", "testuser", Roles.Client);
            _userRepoMock.Setup(x => x.GetUserByUserName("testuser"))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyPassword("wrong", user.PasswordHash))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.Login("testuser", "wrong"));
        }

        [Fact]
        public async Task Register_ValidData_CreatesUserAndClient()
        {
            var userName = "newuser";
            var password = "password123";
            var phone = "+71234567890";
            var firstName = "John";
            var lastName = "Doe";
            string? patronymic = null;
            var passwordHash = "hashedPassword";

            _userRepoMock.Setup(x => x.GetUserByUserName(userName))
                .ReturnsAsync((User?)null);
            _passwordHasherMock.Setup(x => x.Hash(password))
                .ReturnsAsync(passwordHash);
            _clientRepoMock.Setup(x => x.GetByNameAndPhoneAsync(It.IsAny<FullName>(), It.IsAny<PhoneNumber>()))
                .ReturnsAsync((Client?)null);

            await _service.Register(userName, password, phone, firstName, lastName, patronymic);

            _userRepoMock.Verify(x => x.AddUser(It.Is<User>(u =>
                u.UserName == userName && u.Role == Roles.Client)), Times.Once);
            _clientRepoMock.Verify(x => x.AddClient(It.IsAny<Client>()), Times.Once);
            _userRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _clientRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Register_UserNameAlreadyTaken_ThrowsArgumentException()
        {
            var existingUser = new User("hash", "existing", Roles.Client);
            _userRepoMock.Setup(x => x.GetUserByUserName("existing"))
                .ReturnsAsync(existingUser);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.Register("existing", "pass", "+71234567890", "John", "Doe", null));
        }

        [Fact]
        public async Task IsNameAvailable_AvailableName_ReturnsTrue()
        {
            _userRepoMock.Setup(x => x.GetUserByUserName("available"))
                .ReturnsAsync((User?)null);

            var result = await _service.IsNameAvailable("available");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsNameAvailable_TakenName_ReturnsFalse()
        {
            var user = new User("hash", "taken", Roles.Client);
            _userRepoMock.Setup(x => x.GetUserByUserName("taken"))
                .ReturnsAsync(user);

            var result = await _service.IsNameAvailable("taken");

            result.Should().BeFalse();
        }
    }
}