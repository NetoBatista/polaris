using Microsoft.EntityFrameworkCore;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;

namespace Polaris.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly PolarisContext _context;
        public UserRepository(PolarisContext context)
        {
            _context = context;
        }

        public Task<bool> Exists(User user)
        {
            var email = user.Email ?? string.Empty;
            return _context.User.AnyAsync(x => x.Id == user.Id || x.Email.ToUpper() == email.ToUpper());
        }

        public async Task<User> Create(User user)
        {
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public Task<User?> Get(User user)
        {
            var email = user.Email ?? string.Empty;
            return _context.User.FirstOrDefaultAsync(x => x.Email.ToUpper() == email.ToUpper() || x.Id == user.Id);
        }

        public Task<List<User>> GetByApplication(Guid applicationId)
        {
            return _context.User.Include(x => x.MemberNavigation)
                                .Where(x => x.MemberNavigation.Any(y => y.ApplicationId == applicationId))
                                .ToListAsync();
        }

        public Task<List<User>> Get()
        {
            return _context.User.OrderBy(x => x.Email).ToListAsync();
        }

        public async Task<bool> Remove(User user)
        {
            var entity = await _context.User.AsNoTracking()
                                            .Include(x => x.MemberNavigation)
                                            .ThenInclude(x => x.AuthenticationNavigation)
                                            .FirstAsync(x => x.Id == user.Id);
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Update(User user)
        {
            var entity = await _context.User.AsNoTracking().FirstAsync(x => x.Id == user.Id);
            if (!string.IsNullOrEmpty(user.Name))
            {
                entity.Name = user.Name;
            }
            if (!string.IsNullOrEmpty(user.Language))
            {
                entity.Language = user.Language;
            }
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
