using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HexagonalModular.Core.Entities;
using System.Threading.Tasks;

namespace HexagonalModular.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByEmailAsync(string email);
        Task AddAsync(User usuario);
        Task<bool> ExistsByEmailAsync(string email);
  
    }
}

