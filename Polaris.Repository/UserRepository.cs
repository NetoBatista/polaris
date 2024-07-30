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

        public Task<bool> AlreadyCreated(User user)
        {
            return _context.User.AnyAsync(x => x.Id == user.Id || x.Email.ToUpper() == user.Email.ToUpper());
        }

        public async Task<User> Create(User user)
        {
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public Task<User?> Get(User user)
        {
            return _context.User.FirstOrDefaultAsync(x => x.Email.ToUpper() == user.Email.ToUpper());
        }

        public Task<List<User>> Get()
        {
            return _context.User.OrderBy(x => x.Email).ToListAsync();
        }

        public async Task<bool> Remove(User user)
        {
            var entity = await _context.User.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);
            if (entity == null)
            {
                return false;
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Update(User user)
        {
            var entity = await _context.User.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);
            if (entity == null)
            {
                return false;
            }
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
