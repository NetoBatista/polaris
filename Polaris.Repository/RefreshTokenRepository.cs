using Microsoft.EntityFrameworkCore;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;

namespace Polaris.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly PolarisContext _context;
        public RefreshTokenRepository(PolarisContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> Create(Guid authenticationId)
        {
            var entity = new RefreshToken
            {
                AuthenticationId = authenticationId,
                Expiration = DateTime.UtcNow.AddDays(30),
                Token = Guid.NewGuid().ToString(),
            };
            await _context.RefreshToken.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public Task<RefreshToken?> Get(RefreshToken entity)
        {
            return _context.RefreshToken.AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.AuthenticationId == entity.AuthenticationId ||
                                                                  x.Token == entity.Token ||
                                                                  x.Id == entity.Id);
        }

        public async Task Remove(RefreshToken entity)
        {
            var refreshToken = await _context.RefreshToken.AsNoTracking()
                                                          .FirstOrDefaultAsync(x => x.AuthenticationId == entity.AuthenticationId ||
                                                                                    x.Token == entity.Token ||
                                                                                    x.Id == entity.Id);

            _context.RefreshToken.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
