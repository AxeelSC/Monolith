using HexagonalModular.Application.Authentication.Commands;
using HexagonalModular.Application.Authentication.Interfaces;
using HexagonalModular.Application.Security;
using HexagonalModular.Application.Users;
using HexagonalModular.Core.Entities;
using HexagonalModular.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Application.Authentication.UseCases
{
    public class RegisterHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;

        public RegisterHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork,
            IAuthService authService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        public async Task<RegisterResult> HandleAsync(RegisterCommand command)
        {
            if (await _userRepository.ExistsByEmailAsync(command.Email.Value))
                throw new InvalidOperationException("Email already exists");

            return await _authService.RegisterAndLoginAsync(command);
        }
    }
}
