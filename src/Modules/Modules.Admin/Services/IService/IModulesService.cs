using InfrastructureCore;
using InfrastructureCore.Models.Module;
using Modules.Common.Models;
using System.Collections.Generic;

namespace Modules.Admin.Services.IService
{
    public interface IModulesService
    {
        List<SYModulesMg> GetAllModulesBySite();
        Result SavedDataModulesBySite(SYModulesMg item, string action);
        Result DeletedDataModulesBySite(SYModulesMg item);
        Result ApplyModulesConfig();// restart server iis
    }
}
