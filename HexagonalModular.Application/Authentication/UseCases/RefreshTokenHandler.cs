using HexagonalModular.Application.Interfaces.User;
using HexagonalModular.Core.Interfaces__Ports_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Application.Authentication.UseCases
{
    public class RefreshTokenHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RefreshTokenHandler(IUserRepository userRepository, ITokenGenerator tokenGenerator, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<RefreshTokenResponseDto> HandleAsync(RefreshTokenRequestDto request)
        {
            var storedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (storedRefreshToken == null || storedRefreshToken.ExpirationDate < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            var user = await _userRepository.GetByIdAsync(storedRefreshToken.UserId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            var newToken = _tokenGenerator.GenerateToken(user);

            return new RefreshTokenResponseDto
            {
                Token = newToken,
                UserId = user.Id.ToString()
            };
        }
    }
}
