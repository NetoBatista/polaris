using Microsoft.EntityFrameworkCore;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;
using System.Diagnostics.CodeAnalysis;

namespace Polaris.Repository
{
    [ExcludeFromCodeCoverage]
    public class MigrationRepository : IMigrationRepository
    {
        private readonly PolarisContext _context;

        public MigrationRepository(PolarisContext context)
        {
            _context = context;
        }

        public Task Execute()
        {
            return _context.Database.MigrateAsync();
        }

        public Task<IEnumerable<string>> Get()
        {
            return _context.Database.GetPendingMigrationsAsync();
        }
    }
}
