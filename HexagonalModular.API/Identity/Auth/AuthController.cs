using HexagonalModular.API.Common;
using HexagonalModular.API.Identity.Auth.Request;
using HexagonalModular.API.Identity.Auth.Responses;
using HexagonalModular.Application.Identity.Authentication.Login;
using HexagonalModular.Application.Identity.Authentication.Register;
using HexagonalModular.Application.Identity.Authentication.Tokens;
using HexagonalModular.Application.Identity.Common.Ports;
using HexagonalModular.Core.Identity.ValueObjects;
using HexagonalModular.Core.Shared;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HexagonalModular.API.Identity.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginHandler _loginHandler;
        private readonly IRegisterHandler _registerHandler;
        private readonly IRefreshTokenHandler _refreshTokenHandler;
        private readonly ILogger<AuthController> _logger;
        public AuthController(
            ILoginHandler loginHandler,
            IRegisterHandler registerHandler,
            IRefreshTokenHandler refreshTokenHandler,
            ILogger<AuthController> logger)
        {
            _loginHandler = loginHandler;
            _registerHandler = registerHandler;
            _refreshTokenHandler = refreshTokenHandler;
            _logger = logger;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var traceIdInvalid = HttpContext.TraceIdentifier;

                _logger.LogWarning(
                    "Invalid input data in {Endpoint}. TraceId={TraceId}",
                    "POST /api/auth/login",
                    traceIdInvalid);

                return BadRequest(ApiResponse<LoginResponseDto>.ErrorResult(
                    "INVALID_INPUT",
                    "Invalid input data",
                    traceId: traceIdInvalid));
            }

            var emailVo = Email.Create(request.Email);
            var command = new LoginCommand(emailVo, request.Password);

            var result = await _loginHandler.HandleAsync(command);

            var traceId = HttpContext.TraceIdentifier;

            if (result.IsFailure)
            {
                var error = result.Error!;

                _logger.LogWarning(
                     "Login failed in {Endpoint}. Code={ErrorCode}, Message={ErrorMessage}, TraceId={TraceId}",
                     "POST /api/auth/login",
                     error.Code,
                     error.Message,
                     traceId);

                if (error.Code == Errors.Authentication.InvalidCredentials.Code)
                {
                    return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResult(
                        error.Code,
                        "Invalid email or password",
                        traceId: traceId));
                }

                return BadRequest(ApiResponse<LoginResponseDto>.ErrorResult(
                    error.Code,
                    error.Message,
                    traceId: traceId));
            }

            var loginResult = result.Value!;

            var response = new LoginResponseDto
            {
                AccessToken = loginResult.Session.AccessToken,
                RefreshToken = loginResult.Session.RefreshToken,
                User = new UserResponseDto
                {
                    Id = loginResult.Session.User.Id.ToString(),
                    Email = loginResult.Session.User.Email,
                    Name = loginResult.Session.User.Name
                }
            };

            return Ok(ApiResponse<LoginResponseDto>.SuccessResult(
                response,
                "Login successful"
            ));
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<RegisterResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<RegisterResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<RegisterResponseDto>>> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var traceIdInvalid = HttpContext.TraceIdentifier;

                _logger.LogWarning(
                    "Invalid input data in {Endpoint}. TraceId={TraceId}",
                    "POST /api/auth/register",
                    traceIdInvalid);

                return BadRequest(ApiResponse<RegisterResponseDto>.ErrorResult(
                    "INVALID_INPUT",
                    "Invalid input data",
                    traceId: traceIdInvalid));
            }

            var emailVo = Email.Create(request.Email);
            var registerCommand = new RegisterCommand(request.Username, emailVo, request.Password);

            var result = await _registerHandler.HandleAsync(registerCommand);

            var traceId = HttpContext.TraceIdentifier;

            if (result.IsFailure)
            {
                var error = result.Error!;

                _logger.LogWarning(
                    "Register failed in {Endpoint}. Code={ErrorCode}, Message={ErrorMessage}, TraceId={TraceId}",
                    "POST /api/auth/register",
                    error.Code,
                    error.Message,
                    traceId);

                return BadRequest(ApiResponse<RegisterResponseDto>.ErrorResult(
                    error.Code,
                    error.Message,
                    traceId: traceId));
            }

            var registerResult = result.Value!;

            var response = new RegisterResponseDto
            {
                AccessToken = registerResult.Session.AccessToken,
                RefreshToken = registerResult.Session.RefreshToken,
                User = new UserResponseDto
                {
                    Id = registerResult.Session.User.Id.ToString(),
                    Email = registerResult.Session.User.Email,
                    Name = registerResult.Session.User.Name
                }
            };

            return Ok(ApiResponse<RegisterResponseDto>.SuccessResult(
                response,
                "Registration successful"
            ));
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(ApiResponse<RefreshTokenResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<RefreshTokenResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<RefreshTokenResponseDto>), StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult<ApiResponse<RefreshTokenResponseDto>>> RefreshToken(
            [FromBody] RefreshTokenRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var traceIdInvalid = HttpContext.TraceIdentifier;

                _logger.LogWarning(
                    "Invalid input data in {Endpoint}. TraceId={TraceId}",
                    "POST /api/auth/refresh-token",
                    traceIdInvalid);

                return BadRequest(ApiResponse<RefreshTokenResponseDto>.ErrorResult(
                    "INVALID_INPUT",
                    "Invalid input data",
                    traceId: traceIdInvalid));
            }

            var command = new RefreshTokenCommand(request.RefreshToken);

            var result = await _refreshTokenHandler.HandleAsync(command);

            var traceId = HttpContext.TraceIdentifier;

            if (result.IsFailure)
            {
                var error = result.Error!;

                _logger.LogWarning(
                    "Refresh token failed in {Endpoint}. Code={ErrorCode}, Message={ErrorMessage}, TraceId={TraceId}",
                    "POST /api/auth/refresh-token",
                    error.Code,
                    error.Message,
                    traceId);

                if (error.Code == Errors.Authentication.InvalidRefreshTokenCode)
                {
                    return Unauthorized(ApiResponse<RefreshTokenResponseDto>.ErrorResult(
                        error.Code,
                        "Invalid or expired refresh token",
                        traceId: traceId));
                }

                return BadRequest(ApiResponse<RefreshTokenResponseDto>.ErrorResult(
                    error.Code,
                    error.Message,
                    traceId: traceId));
            }

            var refreshResult = result.Value!;

            var response = new RefreshTokenResponseDto
            {
                Token = refreshResult.AccessToken,
                UserId = refreshResult.UserId
            };

            return Ok(ApiResponse<RefreshTokenResponseDto>.SuccessResult(
                response,
                "Token refreshed successfully"
            ));
        }                 
    }
}
