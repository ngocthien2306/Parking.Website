using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Models
{
    public class MES_ProjectProdcnLines
    {
        public string ProjectCode { get; set; }
        public string ProdcnCode { get; set; }
        public string ProdcnLineCode { get; set; }
        public int AssignedQty { get; set; }
        public int ProdDoneQty { get; set; }
        public string LineManager { get; set; }
        public string ProdcnLineState { get; set; }
        public DateTime ProdLineStartDate { get; set; }
        public DateTime ProdLineDoneDate { get; set; }

        //infomation lines
      //  public string ProductLineCode { get; set; }
        public string ProductLineName { get; set; }
        public string ProductLineNameEng { get; set; }
        public string MaterialWarehouseCode { get; set; }
        public string FinishWarehouseCode { get; set; }
        public string InternalExternal { get; set; }
        public string Manager { get; set; }
        public string Status { get; set; }
        public string PartnerCode { get; set; }
    }
}
