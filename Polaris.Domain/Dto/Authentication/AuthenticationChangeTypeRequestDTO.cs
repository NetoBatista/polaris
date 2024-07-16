namespace Polaris.Domain.Dto.Authentication
{
    public class AuthenticationChangeTypeRequestDTO
    {
        public string Email { get; set; } = null!;

        public Guid ApplicationId { get; set; }

        public string Type { get; set; } = string.Empty;

        public string? Password { get; set; }
    }
}
