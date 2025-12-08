using HexagonalModular.Application.Identity.Authentication.Register;
using HexagonalModular.Application.Identity.Common.Persistence;
using HexagonalModular.Application.Identity.Common.Ports;
using HexagonalModular.Application.Identity.Common.Security;
using HexagonalModular.Core.Shared;
using Microsoft.Extensions.Logging;
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
        private readonly IUserRepository _userRepository;
        private readonly IIdentityUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthService _authService;
        private readonly ILogger<LoginHandler> _logger;

        public LoginHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IAuthService authService,
            IIdentityUnitOfWork unitOfWork,
            ILogger<LoginHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _authService = authService;
            _logger = logger;
        }

        public async Task<Result<LoginResult>> HandleAsync(LoginCommand command)
        {
            var user = await _userRepository.GetByEmailAsync(command.Email);
            var traceId = "Unknown";  

            if (user is null)
            {
                _logger.LogWarning(
                    "Login failed: user not found for email {Email}",
                    traceId,
                    command.Email.Value);

                return Result<LoginResult>.Failure(Errors.Authentication.InvalidCredentials());
            }

            if (!_passwordHasher.Verify(command.Password, user.PasswordHash))
            {
                _logger.LogWarning(
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

