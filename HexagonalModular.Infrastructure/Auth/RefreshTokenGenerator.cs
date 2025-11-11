using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using HexagonalModular.Core.Entities;
using Microsoft.Extensions.Configuration;
using HexagonalModular.Application.Authentication.Interfaces;

namespace HexagonalModular.Infrastructure.Auth
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        private readonly int _length;
        private readonly int _daysValid;

        public RefreshTokenGenerator(IConfiguration configuration)
        {
            _length = int.Parse(configuration["Auth:RefreshTokenLength"] ?? "64");
            _daysValid = int.Parse(configuration["Auth:RefreshTokenExpireDays"] ?? "7");
        }

        public RefreshToken Generate(Guid userId)
        {
            var bytes = new byte[_length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            var tokenValue = Convert.ToBase64String(bytes);
            var expiration = DateTime.UtcNow.AddDays(_daysValid);

            return new RefreshToken(userId, tokenValue, expiration);
        }
    }
}
