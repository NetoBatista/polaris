using Polaris.Domain.Entity;

namespace Polaris.Domain.Interface.Repository
{
    public interface IMemberRepository
    {
        Task<Member> Create(Member member);

        Task Remove(Member member);

        Task<List<Member>> Get(Member member);

        Task<bool> Exists(Member member);
    }
}