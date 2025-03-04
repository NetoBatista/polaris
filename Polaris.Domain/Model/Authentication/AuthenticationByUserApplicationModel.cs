namespace Polaris.Domain.Model.Authentication
{
    public class AuthenticationByUserApplicationModel
    {
        public string Email { get; set; } = null!;

        public Guid ApplicationId { get; set; }
    }
}
