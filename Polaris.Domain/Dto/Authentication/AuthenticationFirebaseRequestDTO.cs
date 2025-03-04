namespace Polaris.Domain.Dto.Authentication
{
    public class AuthenticationFirebaseRequestDTO
    {
        public string Email { get; set; } = null!;

        public Guid ApplicationId { get; set; }

        public string FirebaseAppId { get; set; } = null!;

        public string TokenFirebase { get; set; } = null!;

        public string JsonCredentials { get; set; } = null!;
    }
}
