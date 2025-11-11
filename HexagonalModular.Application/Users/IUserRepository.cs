using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HexagonalModular.Core.Entities;
using System.Threading.Tasks;

namespace HexagonalModular.Application.Users
{
    public interface IUserRepository
    {
        Task<UserDomain> GetByIdAsync(Guid id);
        Task<UserDomain> GetByEmailAsync(string email);
        Task AddAsync(UserDomain usuario);
        Task<bool> ExistsByEmailAsync(string email);

    }
}

