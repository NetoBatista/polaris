namespace Polaris.Domain.Entity
{
    public class Authentication
    {
        public Guid Id { get; set; }

        public Guid MemberId { get; set; }

        public string? Password { get; set; }

        public string? RefreshToken { get; set; }

        public string? Code { get; set; }

        public int? CodeAttempt { get; set; }

        public DateTime? CodeExpiration { get; set; }

        public virtual Member MemberNavigation { get; set; } = null!;
    }
}
