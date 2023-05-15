using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels
{
 public   class MES_OrderStatus
    {
        public int NO { get; set; }
        public string POStatus { get; set; }
        public string UserPONumber { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? ArrivalRequestDate { get; set; }
        public DateTime? SlipDate { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string UserProjectCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemUnit { get; set; }
        public int ReceiveQty { get; set; }
        public string MonetaryUnit { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal TransactionAmount { get; set; }
        public string PleigerRemark { get; set; }


        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }







    }
}
