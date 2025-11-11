using HexagonalModular.Application;
using HexagonalModular.Application.Authentication.Commands;
using HexagonalModular.Application.Authentication.Interfaces;
using HexagonalModular.Application.Security;
using HexagonalModular.Core.Entities;
using HexagonalModular.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Infrastructure.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator tokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = tokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
        }
   
        public async Task<LoginResult> LoginAsync(LoginCommand command)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(command.Email.Value)
                ?? throw new UnauthorizedAccessException("Invalid email or password");

            if (!_passwordHasher.Verify(command.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password");

            var session = await CreateAuthSessionAsync(user);

            return new LoginResult(session);
        }

        public async Task<RegisterResult> RegisterAndLoginAsync(RegisterCommand command)
        {
            if (await _unitOfWork.Users.GetByEmailAsync(command.Email.Value) != null)
                throw new InvalidOperationException("Email already in use.");

            var hashedPassword = _passwordHasher.Hash(command.Password);

            var user = new UserDomain(command.Name, command.Email, hashedPassword);

            await _unitOfWork.Users.AddAsync(user);

            var session = await CreateAuthSessionAsync(user);

            return new RegisterResult(session);
        }
        private async Task<AuthSession> CreateAuthSessionAsync(UserDomain user)
        {
            var accessToken = _jwtTokenGenerator.GenerateToken(user);

            var refreshToken = _refreshTokenGenerator.Generate(user.Id);

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);

            await _unitOfWork.CommitAsync();

            var loggedUser = new LoggedUserModel(user.Id, user.Email.Value, user.Name);

            return new AuthSession(accessToken, refreshToken.Token, loggedUser);
        }
    }
}
