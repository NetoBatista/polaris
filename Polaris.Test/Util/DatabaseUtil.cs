using Microsoft.EntityFrameworkCore;
using Polaris.Domain.Entity;

namespace Polaris.Test.Util
{
    public static class DatabaseUtil
    {
        public static PolarisContext Create()
        {
            var dbContextOptions = new DbContextOptionsBuilder<PolarisContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
            return new PolarisContext(dbContextOptions.Options);
        }
    }
}
