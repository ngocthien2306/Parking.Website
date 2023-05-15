using InfrastructureCore.DAL;
using InfrastructureCore.Models.Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InfrastructureCore.Modules
{
    public class ModuleConfigurationManager : IModuleConfigurationManager
    {
        public static readonly string ModulesFilename = "modules.json";

        private static readonly string LOAD_SP_WEB_SYMODULES_MANAGEMENT = "SP_Web_SYModulesMg";
        public IEnumerable<ModuleInfo> GetModules()
        {

            var modulesPath = Path.Combine(GlobalConfiguration.ContentRootPath, ModulesFilename);
            using (var reader = new StreamReader(modulesPath))
            {
                string content = reader.ReadToEnd();
                dynamic modulesData = JsonConvert.DeserializeObject(content);
                foreach (dynamic module in modulesData)
                {
                    yield return new ModuleInfo
                    {
                        Id = module.id,
                        Version = Version.Parse(module.version.ToString()),
                        IsBundledWithHost = module.isBundledWithHost,
                        IsActive = true
                    };
                }
            }
        }
    }
}
