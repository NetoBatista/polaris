namespace Polaris.Domain.Dto.Authentication
{
    public class AuthenticationGenerateCodeRequestDTO
    {
        public string Email { get; set; } = null!;

        public Guid ApplicationId { get; set; }
    }
}
