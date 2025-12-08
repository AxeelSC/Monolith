using HexagonalModular.Application.Identity.Common.Persistence;
using HexagonalModular.Application.Identity.Common.Ports;
using HexagonalModular.Application.Identity.Common.Security;
using HexagonalModular.Core.Identity.Entities;
using HexagonalModular.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Application.Identity.Authentication.Register
{
    public class RegisterHandler : IRegisterHandler
    {
        private readonly IIdentityUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthService _authService;

        public RegisterHandler(
            IIdentityUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _authService = authService;
        }

        public async Task<Result<RegisterResult>> HandleAsync(RegisterCommand command)
        {
            var emailExists = await _unitOfWork.Users.ExistsByEmailAsync(command.Email);

            if (emailExists)
                return Result<RegisterResult>.Failure(Errors.Users.EmailAlreadyInUse(command.Email.Value));

            var hashedPassword = _passwordHasher.Hash(command.Password);
            var user = new UserDomain(command.Name, command.Email, hashedPassword);

            await _unitOfWork.Users.AddAsync(user);

            var session = await _authService.CreateAuthSessionAsync(user);

            await _unitOfWork.CommitAsync();

            var registerResult = new RegisterResult(session);

            return Result<RegisterResult>.Success(registerResult);
        }
    }
}
