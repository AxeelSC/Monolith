using HexagonalModular.Application.Identity.Authentication.Register;
using HexagonalModular.Application.Identity.Common.Persistence;
using HexagonalModular.Application.Identity.Common.Ports;
using HexagonalModular.Application.Identity.Common.Security;
using HexagonalModular.Core.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HexagonalModular.Application.Identity.Authentication.Login
{
    public class LoginHandler : ILoginHandler
    {
        private readonly IAuthService _authService;
        private readonly IIdentityUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;
        private readonly ILoggingService _loggingService;
        public LoginHandler(
             IIdentityUnitOfWork unitOfWork,
             IPasswordHasher passwordHasher,
            IAuthService authService,
            ILoggingService loggingService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _authService = authService;
            _loggingService = loggingService;
        }

        public async Task<Result<LoginResult>> HandleAsync(LoginCommand command)
        {
            var user = await _userRepository.GetByEmailAsync(command.Email.Value);
            var traceId = "Unknown";  

            if (user is null)
            {
                _loggingService.LogWarning(
                    "Login failed: user not found for email {Email}",
                    traceId,
                    command.Email.Value);

                return Result<LoginResult>.Failure(Errors.Authentication.InvalidCredentials());
            }

            if (!_passwordHasher.Verify(command.Password, user.PasswordHash))
            {
                _loggingService.LogWarning(
                    "Login failed: invalid password for user {UserId}",
                    traceId,
                    user.Id);

                return Result<LoginResult>.Failure(Errors.Authentication.InvalidCredentials());
            }

            var session = await _authService.CreateAuthSessionAsync(user);

            await _unitOfWork.CommitAsync();

            return Result<LoginResult>.Success(new LoginResult(session));
        }
    }
}

