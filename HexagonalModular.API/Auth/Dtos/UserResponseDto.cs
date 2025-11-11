namespace HexagonalModular.API.Auth.Dtos
{
    public class UserResponseDto
    {
        public string Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Name { get; set; }
    }
}
