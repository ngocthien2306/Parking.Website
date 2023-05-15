using System.Collections.Generic;

namespace InfrastructureCore.Modules
{
    public interface IModuleConfigurationManager
    {
        IEnumerable<ModuleInfo> GetModules();
    }
}
