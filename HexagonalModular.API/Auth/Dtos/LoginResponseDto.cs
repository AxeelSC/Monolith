namespace HexagonalModular.API.Auth.Dtos
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public UserResponseDto User { get; set; } = default!;
    }
}
