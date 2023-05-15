using Microsoft.EntityFrameworkCore;

namespace InfrastructureCore.Data
{
    public interface ICustomModelBuilder
    {
        void Build(ModelBuilder modelBuilder);
    }
}
