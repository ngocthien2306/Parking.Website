using InfrastructureCore.Configuration;
using InfrastructureCore.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureCore
{
    public static class GlobalConfiguration
    {
        public static IList<ModuleInfo> Modules { get; set; } = new List<ModuleInfo>();

        public static DbConnectionsManager DbConnections { get; private set; } = new DbConnectionsManager();

        //public static IList<Culture> Cultures { get; set; } = new List<Culture>();

        public static string DefaultCulture => "en-US";

        public static string WebRootPath { get; set; }

        public static string ContentRootPath { get; set; }

        public static IList<string> AngularModules { get; } = new List<string>();

        public static void RegisterAngularModule(string angularModuleName)
        {
            AngularModules.Add(angularModuleName);
        }
    }
}
