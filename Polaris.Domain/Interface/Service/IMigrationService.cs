namespace Polaris.Domain.Interface.Service
{
    public interface IMigrationService
    {
        Task<IEnumerable<string>> Get();
        Task Execute();
    }
}
