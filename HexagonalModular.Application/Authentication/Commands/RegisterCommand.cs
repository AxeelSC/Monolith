using HexagonalModular.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Application.Authentication.Commands
{
    public record RegisterCommand(string Name, Email Email, string Password);

}
