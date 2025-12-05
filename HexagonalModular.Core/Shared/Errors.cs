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

            public const string InvalidCredentialsCode = "AUTH.INVALID_CREDENTIALS";
            public const string InvalidRefreshTokenCode = "AUTH.INVALID_REFRESH_TOKEN";
            public const string EmailAlreadyInUseCode = "AUTH.EMAIL_ALREADY_IN_USE";
            public const string UserNotFoundCode = "AUTH.USER_NOT_FOUND";
            public const string AccountLockedCode = "AUTH.ACCOUNT_LOCKED";
            public const string EmailNotConfirmedCode = "AUTH.EMAIL_NOT_CONFIRMED";
            public const string TokenGenerationFailedCode = "AUTH.TOKEN_GENERATION_FAILED";

            public static Error InvalidCredentials() =>
                new(InvalidCredentialsCode, "Invalid email or password.");

            public static Error InvalidRefreshToken(string? token = null) =>
                new(InvalidRefreshTokenCode,
                    token is null
                        ? "Refresh token validation failed."
                        : $"Refresh token validation failed. Token={Truncate(token)}");

            public static Error EmailAlreadyInUse(string email) =>
                new(EmailAlreadyInUseCode,
                    $"Registration failed: email '{email}' is already in use.");

            public static Error UserNotFound(Guid userId) =>
                new(UserNotFoundCode,
                    $"Authentication failed: user '{userId}' not found.");

            public static Error AccountLocked(string email) =>
                new(AccountLockedCode,
                    $"User '{email}' attempted login but account is locked.");

            public static Error EmailNotConfirmed(string email) =>
                new(EmailNotConfirmedCode,
                    $"User '{email}' attempted login but email is not confirmed.");

            public static Error TokenGenerationFailed(string reason) =>
                new(TokenGenerationFailedCode,
                    $"JWT generation failed: {reason}");
        }

        public static class Users
        {
            public const string EmailAlreadyInUseCode = "USER.EMAIL_ALREADY_IN_USE";
            public const string NotFoundByEmailCode = "USER.NOT_FOUND_BY_EMAIL";

            public static Error EmailAlreadyInUse(string email) =>
                new(EmailAlreadyInUseCode,
                    $"User registration failed: email '{email}' is already in use.");

            public static Error NotFoundByEmail(string email) =>
                new(NotFoundByEmailCode,
                    $"User lookup failed: no user found with email '{email}'.");
        }
        private static string Truncate(string value, int max = 10)
            => value.Length <= max ? value : value[..max] + "...";
    }
}
