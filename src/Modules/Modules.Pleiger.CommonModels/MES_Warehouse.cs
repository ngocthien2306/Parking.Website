using InfrastructureCore.Common;

namespace Modules.Pleiger.CommonModels
{
    public class MES_Warehouse : HLCoreCrudModel
    {
        public int NO { get; set; } // for display No in view
        public string WarehouseCode { get; set; }
        public string WarehouseType { get; set; }
        public string WarehouseName { get; set; }
        public string InternalExternal { get; set; }
        public string Manager { get; set; }
        public string Status { get; set; }
        public string PartnerCode { get; set; }

        //USE FOR PAGE MES_ITEM MANAGEMENT IN WH ITEM GRID
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
