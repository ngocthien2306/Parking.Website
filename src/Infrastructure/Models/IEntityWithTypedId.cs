namespace InfrastructureCore.Models
{
    public interface IEntityWithTypedId<TId>
    {
        TId Id { get; }
    }
}
