using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels
{
    public class MES_ProductStatus
    {
        public int No { get; set; }

        public string ProjectCode { get; set; }
        public string ProdcnCode { get; set; }
        public string ProdcnLineCode { get; set; }
        public int AssignedQty { get; set; }
        public int ProdDoneQty { get; set; }
        public string LineManager { get; set; }
        public string ProdcnLineState { get; set; }
        public DateTime? ProdLineStartDate { get; set; }
        public DateTime? ProdLineDoneDate { get; set; }
        public string NameKor { get; set; }
        public string ItemCode { get; set; }
        public string UserProjectCode { get; set; }


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

        // Dat add 2021-1-6
        public int OrderQuantity { get; set; }
        public string ProjectOrderType { get; set; }
        public string SalesOrderProjectCode { get; set; }
        public int DeliveryTotalQty { get; set; }

        // Type WH

        public string WHType { get; set; }

        public int? Internal { get; set; }
        public int? External { get; set; }  
    }
}

