using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Core.Shared
{
    public static class Errors
    {
        public static class Authentication
        {
            public static readonly Error InvalidCredentials =
                new("AUTH.INVALID_CREDENTIALS",
                    "Login failed: invalid credentials in LoginHandler.");


            public const string InvalidRefreshTokenCode = "AUTH.INVALID_REFRESH_TOKEN";
            public static Error InvalidRefreshToken(string? token = null) =>
                new(InvalidRefreshTokenCode,
                    token is null
                        ? "Refresh token validation failed in RefreshTokenHandler."
                        : $"Refresh token validation failed in RefreshTokenHandler. Token={Truncate(token)}");
        }

        public static class Users
        {
            public static Error EmailAlreadyInUse(string email) =>
                new("USER.EMAIL_ALREADY_IN_USE",
                    $"User registration failed: email '{email}' is already in use.");

            public static Error NotFoundByEmail(string email) =>
                new("USER.NOT_FOUND_BY_EMAIL",
                    $"User lookup failed: no user found with email '{email}'.");
        }
        private static string Truncate(string value, int max = 10)
            => value.Length <= max ? value : value[..max] + "...";
    }
}
