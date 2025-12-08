using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HexagonalModular.Application.Identity.Common.Persistence.Entitites;
using HexagonalModular.Core.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace HexagonalModular.Infrastructure.Identity.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
