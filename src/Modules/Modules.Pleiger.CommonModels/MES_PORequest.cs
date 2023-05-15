using System;
using System.Collections.Generic;

namespace Modules.Pleiger.CommonModels
{
    public class MES_PORequest
    {
        public int No { get; set; }
        public bool IsNew { get; set; } // New PO or already exists
        public string ProjectCode { get; set; }
        public string UserProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string RequestCode { get; set; }
        public string PONumber { get; set; }
        public string UserPONumber { get; set; }
        public decimal TotalPrice { get; set; }
        public int RealQty { get; set; }

        public string StatusCode { get; set; }
        public string Status { get; set; }

        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }

        public string ArrivalRequestDate { get; set; }
        public string RealArrivalReqDate { get; set; }

        public string RequestDate { get; set; }
        public string Created_At { get; set; }
        public string Created_By { get; set; }
        
        public string UserRequest { get; set; }

        public string AcceptDate { get; set; }
        public string UserAccept { get; set; }

        public string RejectDate { get; set; }
        public string UserReject { get; set; }

        public List<MES_ItemPO> ListItemPORequest { get; set; }
        public string ListItemPO { get; set; }
        //bao add
        public string ItemCode { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? LeadTimeType { get; set; }
        // add for Sea
        public string OrderConfirmNumber { get; set; }
        public string HullNo { get; set; }
        public string BusinessType { get; set; }
        public string ConnectionToDemand { get; set; }
        public string Yard { get; set; }
        public string Schedule { get; set; }
        public int Mon { get; set; }
        public string SPPR { get; set; }
        public string SPPriceRef { get; set; }
        public string RequestShipMode { get; set; }
        public string FinalShipmentMode { get; set; }
        public string BLCode { get; set; }
        public string Invoice { get; set; }
        public DateTime? InvoiceIssuedDate { get; set; }
        public string PartnerUser { get; set; }
        public string Packing { get; set; }
        public string RefNumber { get; set; }
        public string RemarkAfterConfrimed { get; set; }
        public string RemarkYN { get; set; }

        public String ChangedYN { get; set; }

        /*
            Added By PVN
            Function Name:
            Service Name: 
         */

        public string Supplier { get; set; }

        public string Address { get; set; }

        public string To { get; set; }

        public string OrderNumber { get; set;}

        public DateTime? Date { get; set; }

        public string YourRef { get; set; }

        public string ProjectInfo { get; set; }

        public string Responsible { get; set; }

        public string PhoneNum { get; set; }

        public string TermDelivery { get; set; }

        public string TermPayment { get; set; }

        public  string ItemNO { get; set; }

        public string ItemName { get; set; }

        public string PORemark { get; set; }

        public string PleigerRemark { get; set; }

        public DateTime? DeliveryRequestDate { get; set; }

        public int POQty { get; set; }

        public string Unit { get; set; }

        public string Curency { get; set; }

        public Decimal? UnitPrice { get; set; }
        public Decimal? Total { get; set; }
        public string UserType { get; set; }
        //
        public string UserModify { get; set; }
        public string PhoneNo { get; set; }
        public bool isShowPOOversea { get; set; }
        public string partnerCountry { get; set; }
        public string State { get; set; } //create , detail
        public string SalesClassificationName { get; set; } //create , detail

        public string SalesOrderProjectCode { get; set; }
        public string ProjectOrderType { get; set; } 

    }
}
