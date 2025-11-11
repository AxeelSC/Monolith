using System.ComponentModel.DataAnnotations;

namespace HexagonalModular.API.Auth.Dtos
{
    public class RegisterResponseDto
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
