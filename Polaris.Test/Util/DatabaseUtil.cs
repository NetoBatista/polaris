using Microsoft.EntityFrameworkCore;
using Polaris.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
