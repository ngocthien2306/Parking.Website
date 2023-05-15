using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels
{
    public class MES_ProjectProdcnLines
    {
        public int No { get; set; }
        public string ProjectCode { get; set; }
        public string ProdcnCode { get; set; }
        public string ProdcnLineCode { get; set; }
        public int AssignedQty { get; set; }
        public int ProdDoneQty { get; set; }
        public string LineManager { get; set; }
        public string ProdcnLineState { get; set; }
        public string ProdcnLineStateName { get; set; }
        public DateTime? ProdLineStartDate { get; set; }
        public DateTime? ProdLineEndDate { get; set; }

        public DateTime? ProdLineDoneDate { get; set; }

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

        // Huy add 
        public string OutSource { get; set; }
        public string OutSourceName { get; set; }
        public int PlanRequestQty { get; set; }
        public int GroupLine { get; set; }
        public DateTime? PlanStartDate { get; set; }
        public DateTime? PlanEndDate { get; set; }

        public string LineManagerUsername { get; set; }
        public int LineOrder { get; set; }
        public string Remark { get; set; }

        
    }
}
