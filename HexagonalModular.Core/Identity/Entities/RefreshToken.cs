using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Core.Identity.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid UserId { get; private set; }
        public string Token { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public bool IsRevoked { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public RefreshToken(Guid userId, string token, DateTime expirationDate)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Token = token;
            ExpirationDate = expirationDate;
            IsRevoked = false;
            CreatedAt = DateTime.UtcNow;
        }
        public RefreshToken(Guid id, Guid userId, string token, DateTime expirationDate, bool isRevoked, DateTime createdAt)
        {
            Id = id;
            UserId = userId;
            Token = token;
            ExpirationDate = expirationDate;
            IsRevoked = isRevoked;
            CreatedAt = createdAt;
        }

        public void Revoke() => IsRevoked = true;
    }

}
