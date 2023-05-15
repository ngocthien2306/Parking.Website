using InfrastructureCore.Common;

namespace Modules.Pleiger.Models
{
    public class MES_ItemMonetaryUnit : HLCoreCrudModel
    {
        public int NO { get; set; } // for display No in view
        public string ID { get; set; }
        public string Name { get; set; }
     
    }
}
