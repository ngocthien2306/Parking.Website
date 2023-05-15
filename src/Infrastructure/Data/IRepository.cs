using InfrastructureCore.Models;

namespace InfrastructureCore.Data
{
    public interface IRepository<T> : IRepositoryWithTypedId<T, long> where T : IEntityWithTypedId<long>
    {
    }
}
