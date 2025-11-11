using System.ComponentModel.DataAnnotations;

namespace HexagonalModular.API.Auth.Dtos
{
    public class RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; }
    }
}
