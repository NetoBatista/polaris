namespace Polaris.Domain.Entity
{
    public class Application
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Member> MemberNavigation { get; set; } = [];
    }
}
