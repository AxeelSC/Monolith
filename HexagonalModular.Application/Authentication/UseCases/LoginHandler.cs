using HexagonalModular.Application.Authentication.Commands;
using HexagonalModular.Application.Authentication.Interfaces;
using HexagonalModular.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HexagonalModular.Application.Authentication.UseCases
{
    public class LoginHandler
    {
        private readonly IAuthService _authService;

        public LoginHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<LoginResult> HandleAsync(LoginCommand command)
        {
            return await _authService.LoginAsync(command);
        }
    }
}
