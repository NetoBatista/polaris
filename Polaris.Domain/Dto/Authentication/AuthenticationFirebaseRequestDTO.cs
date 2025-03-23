using System.Text.Json.Serialization;

namespace Polaris.Domain.Dto.Authentication
{
    public class AuthenticationFirebaseRequestDTO
    {
        [JsonIgnore]
        public string Email { get; set; } = string.Empty;

        public Guid ApplicationId { get; set; }

        public string FirebaseAppId { get; set; } = null!;

        public string TokenFirebase { get; set; } = null!;

        public string JsonCredentials { get; set; } = null!;
    }
}
