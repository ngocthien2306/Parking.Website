using System;
using System.Linq;
using System.Reflection;

namespace InfrastructureCore.Modules
{
    public class ModuleInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsBundledWithHost { get; set; }

        public Version Version { get; set; }

        public Assembly Assembly { get; set; }

        public bool IsActive { get; set; }

        //public string SortName
        //{
        //    get
        //    {
        //        return Name.Split('.').Last();
        //    }
        //}

        //public string Path { get; set; }
    }
}
