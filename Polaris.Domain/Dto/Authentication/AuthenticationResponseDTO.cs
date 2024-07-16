namespace Polaris.Domain.Dto.Authentication
{
    public class AuthenticationResponseDTO
    {
        public string Token { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public int Expire { get; set; }

    }
}
