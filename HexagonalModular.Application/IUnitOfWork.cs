using HexagonalModular.Application.Authentication.Interfaces;
using HexagonalModular.Application.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Application
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        Task CommitAsync();
    }
}
