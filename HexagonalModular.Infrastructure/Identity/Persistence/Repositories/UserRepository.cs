
using HexagonalModular.Application.Identity.Common.Persistence.Entitites;
using HexagonalModular.Application.Identity.Common.Ports;
using HexagonalModular.Core.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using CoreEmail = HexagonalModular.Core.Identity.ValueObjects.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HexagonalModular.Core.Shared.Errors;

namespace HexagonalModular.Infrastructure.Identity.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserDomain?> GetByIdAsync(Guid id)
        {
            var entity = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            return entity is null ? null : MapToDomain(entity);
        }

        public async Task<UserDomain?> GetByEmailAsync(CoreEmail email)
        {
            var entity = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.Value.ToLower());

            return entity is null ? null : MapToDomain(entity);
        }

        public async Task AddAsync(UserDomain user)
        {
            var entity = MapToEntity(user);
            await _dbContext.Users.AddAsync(entity);
        }

        public async Task<bool> ExistsByEmailAsync(CoreEmail email)
        {
            return await _dbContext.Users
                .AnyAsync(u => u.Email.ToLower() == email.Value.ToLower());
        }

        private static UserDomain MapToDomain(UserEntity entity)
        {
            return new UserDomain(
                entity.Id,
                entity.Name,
                CoreEmail.Create(entity.Email),
                entity.PasswordHash,
                entity.IsActive   
            );
        }

        private static UserEntity MapToEntity(UserDomain domain)
        {
            return new UserEntity
            {
                Id = domain.Id,
                Email = domain.Email.Value,
                Name = domain.Name,
                PasswordHash = domain.PasswordHash
            };
        }
    }
}
