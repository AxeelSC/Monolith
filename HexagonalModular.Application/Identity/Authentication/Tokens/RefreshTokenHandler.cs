using HexagonalModular.Application.Identity.Common.Ports;
using HexagonalModular.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Application.Identity.Authentication.Tokens
{
    public class RefreshTokenHandler : IRefreshTokenHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RefreshTokenHandler(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<Result<RefreshTokenResult>> HandleAsync(RefreshTokenCommand command)
        {
            var storedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(command.RefreshToken);

            if (storedRefreshToken == null || storedRefreshToken.ExpirationDate < DateTime.UtcNow)
                return Result<RefreshTokenResult>.Failure(Errors.Authentication.InvalidRefreshToken(command.RefreshToken));

            var user = await _userRepository.GetByIdAsync(storedRefreshToken.UserId);

            var newAccessToken = _jwtTokenGenerator.GenerateToken(user);

            var refreshTokenResult = new RefreshTokenResult(newAccessToken, user.Id.ToString());

            return Result<RefreshTokenResult>.Success(refreshTokenResult);
        }
    }
}
