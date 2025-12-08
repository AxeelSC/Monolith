using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Application.Identity.Common.Persistence.Entitites
{
    public class UserEntity
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string PasswordHash { get; set; } = default!;

        public bool IsActive { get; set; }
    }
}
