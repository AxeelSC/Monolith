using HexagonalModular.Application.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Infrastructure.Auth
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string hash, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
