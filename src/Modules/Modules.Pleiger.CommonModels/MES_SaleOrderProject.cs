using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels
{
    public class MES_SaleOrderProject
    {
        public int No { get; set; }
        public string ProjectCodeMaster { get; set; }
        public string UserProjectCodeMaster { get; set; }
        public string UserProject { get; set; }

        public string ProjectNameMaster { get; set; }
        public string PJCodePJName { get; set; }

        public string InCharge { get; set; }
        public string ProductType { get; set; }
        public string ProjectStatus { get; set; }
        public string ProjectStatusName { get; set; }

        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }

        public string PartnerCode { get; set; }
        public string UserCode { get; set; }
        public string PartnerName { get; set; }
        public string ProductTypeName { get; set; }
        public string OrderNumber { get; set; }
        public int OrderQuantity { get; set; }

        public decimal? OrderPrice { get; set; } = 0;
        public decimal? ItemPrice { get; set; } = 0;
        public decimal? ConversionAmount { get; set; } = 0;

        public string DomesticForeign { get; set; }
        public string MonetaryUnit { get; set; }
        public decimal? ExchangeRate { get; set; }
        public DateTime? ExchangeRateDate { get; set; }
        public string VatType { get; set; }
        public decimal? VatRate { get; set; }
        public string OrderTeamCode { get; set; }
        public string OrderTeamCodeName { get; set; }


        public string UserNameRequest { get; set; }
        public string UserIDRequest { get; set; }

        public string RequestCode { get; set; }
        public string RequestType { get; set; }
        public string RequestDate { get; set; }
        public string RequestTypeName { get; set; }
        public string RequestMessage { get; set; }

        public DateTime? PlanDeliveryDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int DeliverQty { get; set; }

        public List<ItemRequest> ListItemRequest { get; set; }
        public List<MES_ItemPO> ListItemPO { get; set; }
        public string ProdcnCode { get; set; }
        public DateTime? ProdcnDate { get; set; }
        public DateTime? PlanDoneDate { get; set; }
        public string ProdcnMakeBy { get; set; }
        public string ProdcnMessage { get; set; }
        public DateTime? ProdcnStartDate { get; set; }
        public DateTime? ProdcnDoneDate { get; set; }
        public DateTime? ProdLineStartDate { get; set; }
        public DateTime? ProdLineDoneDate { get; set; }
        public int ProdcnDoneQty { get; set; }
        public string MaterWHCode { get; set; }
        public string DeliveryType { get; set; }
        public string DeliveryLocation { get; set; }
        public string Manager { get; set; }
        public string ProductLineName { get; set; }

        public string ID { get; set; }
        public string FileMasterID { get; set; }
        public string FileID { get; set; }
        public string UrlPath { get; set; }
        public string Pag_ID { get; set; }
        public string Sp_Name { get; set; }
        public string Pag_Name { get; set; }
        public string Form_Name { get; set; }
        // Quan add del, Upload file set permission
        // 2020/08/18
        public bool Upload_File { get; set; }
        public bool Delele_File { get; set; }
        public bool Btn_Save { get; set; }
        public bool Btn_Delete { get; set; }

        public decimal? Cost { get; set; }
        public decimal? Amt { get; set; }
        public decimal? Tax { get; set; }
        public decimal? TaxAmt { get; set; }

        public string LineManager { get; set; }
        public string AsignedQty { get; set; }
        public string WHToCode { get; set; }
        public string WHToName { get; set; }
        public string WHFromCode { get; set; }
        public string WHFromName { get; set; }
        // Quan add 2021-01-20
        public string SalesClassification { get; set; }
        public string SalesClassificationName { get; set; }
        public string ETC { get; set; }
        // Quan add 2021-02-04
        public DateTime? Created_At { get; set; }
    }
}
