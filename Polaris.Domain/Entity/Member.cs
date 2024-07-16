namespace Polaris.Domain.Entity
{
    public class Member
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid ApplicationId { get; set; }

        public virtual Application ApplicationNavigation { get; set; } = null!;

        public virtual User UserNavigation { get; set; } = null!;

        public virtual Authentication AuthenticationNavigation { get; set; } = null!;
    }
}
