namespace Polaris.Domain.Entity
{
    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Language { get; set; } = null!;

        public virtual ICollection<Member> MemberNavigation { get; set; } = [];
    }
}
