using Microsoft.EntityFrameworkCore;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;

namespace Polaris.Repository;

public class RefreshTokenRepository: IRefreshTokenRepository
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
        };
        await _context.RefreshToken.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public Task<RefreshToken?> Get(Guid id)
    {
        return _context.RefreshToken.FirstOrDefaultAsync(x => !x.Used && (x.Id == id || x.AuthenticationId == id));
    }

    public async Task<bool> Update(Guid id)
    {
        var entity = await _context.RefreshToken.FirstOrDefaultAsync(x => x.Id == id || x.AuthenticationId == id);
        if (entity == null)
        {
            return false;
        }
        
        entity.Used = true;
        _context.RefreshToken.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }
}