using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Polaris.Repository
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly PolarisContext _context;
        public ApplicationRepository(PolarisContext context)
        {
            _context = context;
        }

        public async Task<Application> Create(Application application)
        {
            await _context.Application.AddAsync(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<Application?> Update(Application application)
        {
            var entity = await _context.Application.AsNoTracking().FirstOrDefaultAsync(x => x.Id == application.Id);
            if (entity == null)
            {
                return null;
            }
            entity.Name = application.Name;
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Remove(Application application)
        {
            var entity = await _context.Application.AsNoTracking().FirstOrDefaultAsync(x => x.Id == application.Id);
            if (entity == null)
            {
                return false;
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<List<Application>> GetAll()
        {
            return _context.Application.OrderBy(x => x.Name).ToListAsync();
        }

        public Task<bool> AlreadyCreated(Application application)
        {
            return _context.Application.AnyAsync(x => x.Name.ToUpper() == application.Name.ToUpper() &&
                                                      x.Id != application.Id);
        }

        public Task<bool> Exists(Application application)
        {
            return _context.Application.AnyAsync(x => x.Id == application.Id);
        }

        public Task<bool> AnyMember(Application application)
        {
            return _context.Application.Include(x => x.MemberNavigation)
                                       .AnyAsync(x => x.Id == application.Id && x.MemberNavigation.Count != 0);
        }
    }
}
