using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using HexagonalModular.Core.Identity.Entities;
using HexagonalModular.Core.Identity.ValueObjects;

namespace HexagonalModular.Application.Identity.Common.Ports
{
    public interface IUserRepository
    {
        Task<UserDomain> GetByIdAsync(Guid id);
        Task<UserDomain> GetByEmailAsync(Email email);
        Task AddAsync(UserDomain usuario);
        Task<bool> ExistsByEmailAsync(Email email);

    }
}

