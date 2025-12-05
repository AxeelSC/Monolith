using FluentAssertions;
using HexagonalModular.Application.Identity.Authentication.Login;
using HexagonalModular.Application.Identity.Common.Ports;
using HexagonalModular.Application.Identity.Common.Security;
using HexagonalModular.Core.Identity.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.UnitTest.Identity.Authentication.Login
{
    namespace HexagonalModular.UnitTests.Identity.Authentication.Login
    {
        public class LoginHandlerTests
        {
            private readonly Mock<IUserRepository> _userRepositoryMock;
            private readonly Mock<IPasswordHasher> _passwordHasherMock;
            private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
            private readonly LoginHandler _handler;

            public LoginHandlerTests()
            {
                // Mocks
                _userRepositoryMock = new Mock<IUserRepository>();
                _passwordHasherMock = new Mock<IPasswordHasher>();
                _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();

                // Handler to test
                _handler = new LoginHandler(
                    _userRepositoryMock.Object,
                    _passwordHasherMock.Object,
                    _jwtTokenGeneratorMock.Object
                );
            }
            [Fact]
            public async Task HandleAsync_ShouldReturnFailure_WhenUserNotFound()
            {
                // Arrange
                var command = new LoginCommand("nonexistent@example.com", "password123");

                // Simulamos que el usuario no existe
                _userRepositoryMock
                    .Setup(r => r.GetByEmailAsync("nonexistent@example.com"))
                    .ReturnsAsync((UserDomain?)null);

                // Act
                var result = await _handler.HandleAsync(command);

                // Assert
                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("AUTH.INVALID_CREDENTIALS");
                result.Error.Message.Should().Be("Login failed: invalid credentials for email 'nonexistent@example.com'.");
            }
            [Fact]
            public async Task HandleAsync_ShouldReturnFailure_WhenPasswordIsIncorrect()
            {
                // Arrange
                var command = new LoginCommand("user@example.com", "wrongpassword");

                var user = new UserDomain("User", "user@example.com", "hashedpassword123");
                _userRepositoryMock
                    .Setup(r => r.GetByEmailAsync("user@example.com"))
                    .ReturnsAsync(user);

                // Simulamos que la contraseña no coincide
                _passwordHasherMock
                    .Setup(p => p.Verify("wrongpassword", user.PasswordHash))
                    .Returns(false);

                // Act
                var result = await _handler.HandleAsync(command);

                // Assert
                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("AUTH.INVALID_CREDENTIALS");
                result.Error.Message.Should().Be("Login failed: invalid credentials for email 'user@example.com'.");
            }
            [Fact]
            public async Task HandleAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
            {
                // Arrange
                var command = new LoginCommand("user@example.com", "correctpassword");

                var user = new UserDomain("User", "user@example.com", "hashedpassword123");

                _userRepositoryMock
                    .Setup(r => r.GetByEmailAsync("user@example.com"))
                    .ReturnsAsync(user);

                _passwordHasherMock
                    .Setup(p => p.Verify("correctpassword", user.PasswordHash))
                    .Returns(true);

                var expectedToken = "validToken";
                _jwtTokenGeneratorMock
                    .Setup(j => j.GenerateToken(user))
                    .Returns(expectedToken);

                // Act
                var result = await _handler.HandleAsync(command);

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Session..Should().Be(expectedToken);
                result.Value.User.Id.Should().Be(user.Id);
                result.Value.User.Email.Should().Be(user.Email.Value);
            }
        }
    }
}