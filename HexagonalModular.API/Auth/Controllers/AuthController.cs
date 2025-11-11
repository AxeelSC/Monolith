using HexagonalModular.API.Auth;
using HexagonalModular.API.Auth.Dtos;
using HexagonalModular.API.DTOs.Auth;
using HexagonalModular.API.Models;
using HexagonalModular.Application.Authentication.Commands;
using HexagonalModular.Application.Authentication.UseCases;
using HexagonalModular.Core.Entities;
using HexagonalModular.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HexagonalModular.API.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly LoginHandler _loginHandler;
        private readonly RegisterHandler _registerHandler;
        private readonly RefreshTokenHandler _refreshTokenHandler;

        public AuthController(
            LoginHandler loginHandler,
            RegisterHandler registerHandler,
            RefreshTokenHandler refreshTokenHandler)
        {
            _loginHandler = loginHandler;
            _registerHandler = registerHandler;
            _refreshTokenHandler = refreshTokenHandler;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<LoginResponseDto>.ErrorResult("Invalid input data"));

            try
            {
                var emailVo = Email.Create(request.Email);
                var command = new LoginCommand(emailVo, request.Password);
                var result = await _loginHandler.HandleAsync(command);

                var response = new LoginResponseDto
                {
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken,
                    User = new UserResponseDto
                    {
                        Id = result.User.Id.ToString(),
                        Email = result.User.Email,
                        Name = result.User.Name
                    }
                };

                return Ok(ApiResponse<LoginResponseDto>.SuccessResult(response, "Login successful"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResult(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<LoginResponseDto>.ErrorResult("Internal server error"));
            }
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<RegisterResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<ActionResult<ApiResponse<RegisterResponseDto>>> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<RegisterResponseDto>.ErrorResult("Invalid input data"));
            }
            var emailVO = Email.Create(request.Email);

            var registerCommand = new RegisterCommand(request.Username, emailVO.Value, request.Password);

            var result = await _registerHandler.HandleAsync(registerCommand);

            return Ok(ApiResponse<RegisterResponseDto>.SuccessResult(response, "Registration successful"));
        }
    }
}
