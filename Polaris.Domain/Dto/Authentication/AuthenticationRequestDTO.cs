namespace Polaris.Domain.Dto.Authentication
{
    public class AuthenticationRequestDTO
    {
        public string Email { get; set; } = null!;

        public Guid ApplicationId { get; set; }

        public string? Password { get; set; }

        public string? Code { get; set; }
    }
}
