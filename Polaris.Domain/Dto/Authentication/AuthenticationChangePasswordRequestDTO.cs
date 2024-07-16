namespace Polaris.Domain.Dto.Authentication
{
    public class AuthenticationChangePasswordRequestDTO
    {
        public string Email { get; set; } = null!;

        public Guid ApplicationId { get; set; }

        public string Password { get; set; } = null!;

        public string CurrentPassword { get; set; } = null!;
    }
}
