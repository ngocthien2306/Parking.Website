using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Models
{
    public class MES_ProductLine
    {
        public string ProductLineCode { get; set; }
        public string ProductLineName { get; set; }
        public string ProductLineNameEng { get; set; }
        public string MaterialWarehouseCode { get; set; }
        public string FinishWarehouseCode { get; set; }
        public string InternalExternal { get; set; }
        public string Manager { get; set; }
        public string Status { get; set; }
        public string PartnerCode { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_At { get; set; }
        public string Updated_By { get; set; }
        public DateTime Updated_At { get; set; }
    }
}
