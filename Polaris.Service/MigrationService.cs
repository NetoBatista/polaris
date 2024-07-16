using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Interface.Service;
using System.Diagnostics.CodeAnalysis;

namespace Polaris.Service
{
    [ExcludeFromCodeCoverage]
    public class MigrationService : IMigrationService
    {
        private readonly IMigrationRepository _migrationRepository;

        public MigrationService(IMigrationRepository migrationRepository)
        {
            _migrationRepository = migrationRepository;
        }

        public Task Execute() => _migrationRepository.Execute();

        public Task<IEnumerable<string>> Get() => _migrationRepository.Get();
    }
}
