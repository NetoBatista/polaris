namespace Polaris.Domain.Interface.Repository
{
    public interface IMigrationRepository
    {
        Task<IEnumerable<string>> Get();
        Task Execute();
    }
}
