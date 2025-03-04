using Polaris.Domain.Interface.Service;
using System.Diagnostics.CodeAnalysis;

namespace Polaris.Extension
{
    [ExcludeFromCodeCoverage]
    public static class MigrationExtension
    {
        public static void ExecuteMigrations(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var migrationService = provider.GetService<IMigrationService>();
            if (migrationService == null)
            {
                return;
            }
            var pendingMigrations = migrationService.Get().Result;
            if (pendingMigrations.Any())
            {
                migrationService.Execute().GetAwaiter().GetResult();
            }
        }
    }
}
