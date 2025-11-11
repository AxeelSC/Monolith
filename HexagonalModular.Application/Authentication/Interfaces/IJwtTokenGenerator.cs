using HexagonalModular.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Application.Authentication.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(UserDomain user);
    }
}
