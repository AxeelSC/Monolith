using HexagonalModular.Application.Identity.Common.Persistence.Entitites;
using HexagonalModular.Application.Identity.Common.Ports;
using HexagonalModular.Core.Identity.Entities;
using HexagonalModular.Infrastructure.Identity.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Infrastructure.Identity.Persistence.Repositories
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
            var entity = MapToEntity(refreshToken);
            await _dbContext.RefreshTokens.AddAsync(entity);
        }
        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            var entity = await _dbContext.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(rt =>
                    rt.Token == token &&
                    !rt.IsRevoked &&
                    rt.ExpirationDate > DateTime.UtcNow);

            return entity is null ? null : MapToDomain(entity);
        }
        public async Task RevokeAsync(string token)
        {
            var entity = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);

            if (entity is null)
                return;

            entity.IsRevoked = true;
        }
        private static RefreshToken MapToDomain(RefreshTokenEntity entity)
        {
            
            return new RefreshToken(
                entity.Id,
                entity.UserId,
                entity.Token,
                entity.ExpirationDate,
                entity.IsRevoked,
                entity.CreatedAt
            );
        }

        private static RefreshTokenEntity MapToEntity(RefreshToken domain)
        {
            return new RefreshTokenEntity
            {
                Id = domain.Id,
                UserId = domain.UserId,
                Token = domain.Token,
                ExpirationDate = domain.ExpirationDate,
                IsRevoked = domain.IsRevoked,
                CreatedAt = domain.CreatedAt
            };
        }
    }
}
