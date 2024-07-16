using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;
namespace Polaris.Repository
{
    public class MemberRepository : IMemberRepository
    {
        private readonly PolarisContext _context;
        public MemberRepository(PolarisContext context)
        {
            _context = context;
        }

        public async Task<Member> Create(Member member)
        {
            await _context.AddAsync(member);
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task Remove(Member member)
        {
            var entity = await _context.Member.AsNoTracking().Include(x => x.AuthenticationNavigation)
                                                             .FirstOrDefaultAsync(x => x.Id == member.Id);
            if (entity == null)
            {
                return;
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task<List<Member>> Get(Member member)
        {
            return _context.Member.AsNoTracking()
                                  .Include(x => x.ApplicationNavigation)
                                  .Include(x => x.AuthenticationNavigation)
                                  .Include(x => x.UserNavigation)
                                  .Where(x => x.ApplicationId == member.ApplicationId ||
                                              x.UserId == member.UserId ||
                                              x.Id == member.Id)
                                  .ToListAsync();
        }

        public Task<bool> Exists(Member member)
        {
            return _context.Member.AnyAsync(x => x.ApplicationId == member.ApplicationId &&
                                                 x.UserId == member.UserId);
        }
    }
}