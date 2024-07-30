using Microsoft.EntityFrameworkCore;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;

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

        public async Task<Application> Update(Application application)
        {
            var entity = await _context.Application.AsNoTracking().FirstAsync(x => x.Id == application.Id);
            entity.Name = application.Name;
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Remove(Application application)
        {
            var entity = await _context.Application.AsNoTracking().FirstAsync(x => x.Id == application.Id);
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<List<Application>> GetAll()
        {
            return _context.Application.OrderBy(x => x.Name).ToListAsync();
        }

        public Task<bool> Exists(Application application)
        {
            return _context.Application.AnyAsync(x => x.Id == application.Id || x.Name.ToUpper() == application.Name.ToUpper());
        }

        public Task<bool> NameAlreadyExists(Application application)
        {
            return _context.Application.AnyAsync(x => x.Id != application.Id && x.Name.ToUpper() == application.Name.ToUpper());
        }

        public Task<bool> AnyMember(Application application)
        {
            return _context.Application.Include(x => x.MemberNavigation)
                                       .AnyAsync(x => x.Id == application.Id && x.MemberNavigation.Count != 0);
        }
    }
}
