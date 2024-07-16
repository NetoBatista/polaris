namespace Polaris.Domain.Model
{
    public class AuthenticationByUserApplicationModel
    {
        public string Email { get; set; } = null!;

        public Guid ApplicationId { get; set; }
    }
}
