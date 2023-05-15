using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels
{
    public class MES_SalesOrderProjectNew
    {
        public int No { get; set; }

        public string ProjectOrderType { get; set; }
        public string SalesOrderProjectCode { get; set; }
        public string UserSalesOrderProjectCode { get; set; }    
        public string SalesOrderProjectName { get; set; }
        public string InCharge { get; set; }
        public string OrderTeamCode { get; set; }
        public string OrderTeamName { get; set; }
        public string PartnerCode { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? EditDate { get; set; }
        public string EditUser { get; set; }
        public string ETC { get; set; }
        public string SalesOrderStatus { get; set; }
        public string Customer { get; set; }
        public decimal? TotalOrderPrice { get; set; }
        public int? TotalOrderQuantity { get; set; }
        public string UserName { get; set; }
        public string TeamCode { get; set; }
        public string Check { get; set; }
        public string TypeName { get; set; }
        public string IsDelete { get; set; }
        public string saleOrderProjectCode { get; set; }
        
    }
}
