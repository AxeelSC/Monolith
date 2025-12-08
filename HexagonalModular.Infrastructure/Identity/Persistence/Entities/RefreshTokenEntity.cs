using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Application.Identity.Common.Persistence.Entitites
{
    public class RefreshTokenEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Token { get; set; } = default!;

        public DateTime ExpirationDate { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime CreatedAt { get; set; }
        // Navegación opcional
        public UserEntity? User { get; set; }
    }
}
