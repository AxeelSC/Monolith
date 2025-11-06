using HexagonalModular.Core.Entities;
using HexagonalModular.Core.Interfaces;
using HexagonalModular.Core.Interfaces__Ports_;
using HexagonalModular.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Application.UseCases
{
    public class UserManagerService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public UserManagerService(IUserRepository userRepository, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<User> RegisterUserAsync(string name, string email, string password)
        {
            if (await _userRepository.ExistsByEmailAsync(email))
                throw new Exception("Email is already registered");

            var emailVO = Email.Create(email);
            var passwordHash = _passwordHasher.HashPassword(password);
            var user = new User(name, emailVO, passwordHash);

            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();
            return user;
        }

        // Aquí puedo agregar métodos para login, cambiar password, etc.
    }
}
