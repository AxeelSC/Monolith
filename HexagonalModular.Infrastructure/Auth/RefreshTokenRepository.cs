using HexagonalModular.Application.Authentication.Interfaces;
using HexagonalModular.Core.Entities;
using HexagonalModular.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Infrastructure.Auth
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _dbContext;

        public RefreshTokenRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked && rt.ExpirationDate > DateTime.UtcNow);
        }

        public async Task RevokeAsync(string token)
        {
            var refreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
            if (refreshToken != null)
            {
                refreshToken.Revoke();
            }
        }
    }
}
