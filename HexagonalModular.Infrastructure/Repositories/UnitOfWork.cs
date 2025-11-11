using HexagonalModular.Application;
using HexagonalModular.Application.Authentication.Interfaces;
using HexagonalModular.Application.Users;
using HexagonalModular.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        public IUserRepository Users { get; }
        public IRefreshTokenRepository RefreshTokens { get; }

        public UnitOfWork(AppDbContext dbContext, IUserRepository users, IRefreshTokenRepository refreshTokens)
        {
            _dbContext = dbContext;
            Users = users;
            RefreshTokens = refreshTokens;
        }

        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
