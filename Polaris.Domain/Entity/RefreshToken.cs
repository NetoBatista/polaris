namespace Polaris.Domain.Entity
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public Guid AuthenticationId { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime Expiration { get; set; }

        public virtual Authentication AuthenticationNavigaton { get; set; } = null!;
    }
}
