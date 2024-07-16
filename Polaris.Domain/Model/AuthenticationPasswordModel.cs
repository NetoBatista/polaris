namespace Polaris.Domain.Model
{
    public class AuthenticationPasswordModel
    {
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public Guid ApplicationId { get; set; }
    }
}
