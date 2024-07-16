using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

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
            var entity = await _context.User.FirstOrDefaultAsync(x => x.Id == user.Id || x.Email.ToUpper() == user.Email.ToUpper());
            if (entity != null)
            {
                return entity;
            }
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public Task<User?> Get(User user)
        {
            return _context.User.FirstOrDefaultAsync(x => x.Email.ToUpper() == user.Email.ToUpper());
        }

        public Task<bool> Exists(User user)
        {
            return _context.User.AnyAsync(x => x.Id == user.Id);
        }

        public async Task Remove(User user)
        {
            var entity = await _context.User.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);
            if (entity == null)
            {
                return;
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<User> Update(User user)
        {
            var entity = await _context.User.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);
            if (entity == null)
            {
                return user;
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
            return entity;
        }
    }
}
