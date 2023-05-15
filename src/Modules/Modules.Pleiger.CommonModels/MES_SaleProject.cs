using Modules.Common.Models;
using System;
using System.Collections.Generic;

namespace Modules.Pleiger.CommonModels
{
    public class MES_SaleProject
    {
        public int No { get; set; }
        public string ProjectCode { get; set; }
        public string UserProjectCode { get; set; }
        public string UserSalesOrderProjectCode { get; set; }
        public string UserProject { get; set; }

        public string OrderTeamName { get; set; }
        
        public string ProjectName { get; set; }
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
        public bool Upload_File { get; set; }
        public bool Delele_File { get; set; }       
        public bool Btn_Save { get; set; }
        public bool Btn_Delete { get; set; }      

        public decimal? Cost { get; set; }
        public decimal? Amt { get; set; }
        public decimal? Tax { get; set; }
        public decimal? TaxAmt { get; set; }

      
        public string AsignedQty { get; set; }
        public string WHToCode { get; set; }
        public string WHToName { get; set; }
        public string WHFromCode { get; set; }
        public string WHFromName { get; set; }
        public string SalesClassification { get; set; }
        public string SalesClassificationName { get; set; }
        public string ETC { get; set; }
        // Quan add 2021-02-04
        public DateTime? Created_At { get; set; }

        //Phong add 2021-12-09
        public string ProjectCodeMaster { get; set; }
        public List<SYFileUpload> FileDetail { get; set; }

        //Thien add 2021-12-29
        public string ProjectOrderType { get; set; }
        public string ProjectOrderTypeName { get; set; }
        public bool InitialCode { get; set; }
        public string CustomerName { get; set; }

        //Sales Order Project
        public string SalesOrderProjectName { get; set; }
        public string SalesOrderProjectCode { get; set; }
        public string OrderNumberSalesOrderProject { get; set; }
        public string OrderTeamCodeSalesOrderProject { get; set; }
        public string InChargeSalesOrderProject { get; set; }
        public string Customer { get; set; }
        public int DeliveryTotalQty { get; set; }
        public decimal SalesAmount { get; set; }
        public DateTime? Plan_End_Date { get; set; }
        public DateTime? Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public string Status { get; set; }
        public string Line { get; set; }
        public string LineManager { get; set; }
        public int GroupLine { get; set; }
        public int LineOrder { get; set; }
        public string OutSource { get; set; }
        public string OrderItemName { get; set; }
        public int? OrderQty { get; set; }
        public DateTime? ProductStartDate { get; set; }
        public string OrderTeam { get; set; }
        public DateTime? ProductCompleteDate { get; set; }
        public decimal TotalOrderPrice { get; set; }

        
    }
    public class MES_SaleProjectExCel
    {
        public int No { get; set; }
        public string UserSalesOrderProjectCode { get; set; }
        public DateTime? Created_At { get; set; }
        public string SalesClassification { get; set; }
        public string SalesClassificationName { get; set; }

        public string ETC { get; set; }
        public string ProjectCode { get; set; }
        public bool InitialCode { get; set; }
        public string UserProject { get; set; }
        public string UserProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ProjectStatus { get; set; }
        public string ProjectStatusName { get; set; }
        public string InCharge { get; set; }
        public DateTime? PlanDeliveryDate { get; set; }
        public string ProductType { get; set; }
        public string ItemCode { get; set; }
        public string PartnerCode { get; set; }
        public string OrderNumber { get; set; }
        public string DomesticForeign { get; set; }
        public int OrderQuantity { get; set; }
        public string MonetaryUnit { get; set; }
        public decimal? OrderPrice { get; set; } = 0;
        public decimal? ExchangeRate { get; set; }
        public DateTime? ExchangeRateDate { get; set; }
        public string VatType { get; set; }
        public decimal? VatRate { get; set; }
        public string OrderTeamCode { get; set; }
        public string RequestCode { get; set; }
        public string RequestDate { get; set; }
        public string RequestType { get; set; }
        public string UserNameRequest { get; set; }
        public string UserIDRequest { get; set; }
        public string RequestMessage { get; set; }
        public string PartnerName { get; set; }
        public string ItemName { get; set; }
        public string RequestTypeName { get; set; }
        public string OrderTeamCodeName { get; set; }
        public decimal? ItemPrice { get; set; } = 0;
        public decimal? ConversionAmount { get; set; } = 0;
        public string SalesOrderProjectName { get; set; }
        public string ProjectOrderType { get; set; }
        public int DeliverQty { get; set; }
        public DateTime? DeliveryDate { get; set; }

    }
    public class ItemRequest
    {
        public int?  No{ get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int? ReqQty { get; set; }
        public int StkQty { get; set; }
        public int? POQty { get; set; }
        public int? POFnQty { get; set; }
        public int? totalPOQty { get; set; }
        public int? totalPOFnQty { get; set; }
        public int? RealQty { get; set; }
        public string Category { get; set; }
        public string CategoryName { get; set; }
        public string ItemClassCode { get; set; }
        public string NameEng { get; set; }       

    }

    public class POArrivalRequestDate
    {
        public string PartnerCode { get; set; }
        public string PONumber { get; set; }
        public string ArrivalRequestDate { get; set; }
    }    
    public class MES_MeterialToUpdateWH_EN
    {
        [ColumName("No")]
        public int No { get; set; }

        [ColumName("Category")]
        public string CategoryName { get; set; }

        [ColumName("Item Class Code")]
        public string ItemClassCode { get; set; }

        [ColumName("Item Code")]
        public string ItemCode { get; set; }

        [ColumName("Item Name")]
        public string ItemName { get; set; }

        [ColumName("Real Inventory Qty")]
        public int? RealQty { get; set; }

        [ColumName("Request Qty")]
        public int? ReqQty { get; set; }

        [ColumName("Note")]
        public string Note { get; set; }
    }
    public class MES_MeterialToUpdateWH_KO
    {
        [ColumName("No")]
        public int No { get; set; }

        [ColumName("품목분류")]
        public string CategoryName { get; set; }

        [ColumName("품목구분코드")]
        public string ItemClassCode { get; set; }

        [ColumName("품번")]
        public string ItemCode { get; set; }

        [ColumName("품명")]
        public string ItemName { get; set; }

        [ColumName("재고수량")]
        public int? RealQty { get; set; }

        [ColumName("수량")]
        public int? ReqQty { get; set; }

        [ColumName("메모")]
        public string Note { get; set; }
    }

    public class MES_SaleOrderProjectExcelTemplate
    {
        [ColumName("Project Order Type")]
        public string ProjectOrderType { get; set; }
        [ColumName("User Project Code")]
        public string UserSalesOrderProjectCode { get; set; }
        [ColumName("Project Name")]
        public string SalesOrderProjectName { get; set; }
        [ColumName("In Charge")]
        public string InCharge { get; set; }
        [ColumName("Order Team")]
        public string OrderTeamCode { get; set; }
        [ColumName("Customer")]
        public string PartnerCode { get; set; }
        [ColumName("Customer Order Number")]
        public string OrderNumber { get; set; }
        [ColumName("ETC")]
        public string ETC { get; set; }
    }

    public class MES_ItemManagementExcelTemplate
    {
        [ColumName("Item Class")]
        public string ItemClassCode { get; set; }
        [ColumName("Processing Classification")]
        public string ProcessingClassification { get; set; }
        [ColumName("Item Code")]
        public string ItemCode { get; set; }
        [ColumName("Name Kor")]
        public string NameKor { get; set; }
        [ColumName("Name Eng")]
        public string NameEng { get; set; }
        [ColumName("Safety Quantity")]
        public string SafetyQuantity { get; set; }
        [ColumName("Standard")]
        public string Standard { get; set; }
        [ColumName("Unit")]
        public string Unit { get; set; }
        [ColumName("Inspection Method")]
        public string InspectionMethod { get; set; }       
    }

    public class MES_BomItemExcelTamplateEn
    {
        [ColumName("Item Code")]
        public string ItemCode { get; set; }
        [ColumName("Item Name")]
        public string NameKor { get; set; }
        [ColumName("Need Qty")]
        public string Qty { get; set; }
    }

    public class MES_BomItemExcelTamplateKo
    {
        [ColumName("품번")]
        public string ItemCode { get; set; }
        [ColumName("품명")]
        public string NameKor { get; set; }
        [ColumName("필요수량")]
        public string Qty { get; set; }
    }



    public class MES_SaleProjectExcelTempalte
    {
        [ColumName("Sales Order Project")]
        public string SalesOrderProjectCode { get; set; }
        [ColumName("Sales Classification")]
        public string SalesClassification { get; set; }
        [ColumName("User Project Code")]
        public string UserProjectCode { get; set; }
        [ColumName("Project Name")]
        public string ProjectName { get; set; }
        [ColumName("In Charge")]
        public string InCharge { get; set; }
        [ColumName("Plan Delivery Date")]
        public DateTime? PlanDeliveryDate { get; set; }
        [ColumName("Product")]
        public string ItemCode { get; set; }
        //[ColumName("Customer")]
        //public string PartnerCode { get; set; }
        [ColumName("Order Number")]
        public string OrderNumber { get; set; }
        [ColumName("Domestic Foreign")]
        public string DomesticForeign { get; set; }
        [ColumName("Order Quantity")]
        public int OrderQuantity { get; set; }
        [ColumName("Monetary Unit")]
        public string MonetaryUnit { get; set; }
        [ColumName("Order Price")]
        public decimal? OrderPrice { get; set; }
        [ColumName("Item Price")]
        public decimal? ItemPrice { get; set; }
        [ColumName("Vat Type")]
        public string VatType { get; set; }
        [ColumName("Vat Rate")]
        public decimal? VatRate { get; set; }
        [ColumName("Exchange Rate")]
        public decimal? ExchangeRate { get; set; }
        [ColumName("Exchange Rate Date")]
        public DateTime? ExchangeRateDate { get; set; }
        [ColumName("Order Team")]
        public string OrderTeam { get; set; }
        [ColumName("ETC")]
        public string ETC { get; set; }

    }
   
}
public class MES_ItemPartListExcelTempalte
{
    [ColumName("No")]
    public int? No { get; set; }
    [ColumName("Item Code")]
    public string ItemCode { get; set; }
    [ColumName("Item Name")]
    public string ItemName { get; set; }
    [ColumName("Item Price")]
    public decimal? ItemPrice { get; set; }
    [ColumName("Lead Time")]
    public string LeadTime { get; set; }
    [ColumName("Lead Time Type")]
    public string LeadTimeType { get; set; }
    [ColumName("Monetary Unit")]
    public string MonetaryUnit { get; set; }
    [ColumName("Partner Name")]
    public string PartnerName { get; set; }
    [ColumName("Partner Code")]
    public string PartnerCode { get; set; }
    [ColumName("PO Qty")]
    public int? POQty { get; set; }
    [ColumName("PFE Delivery Date")]
    public DateTime? ArrivalRequestDate { get; set; }
    [ColumName("PFE Remark 1")]
    public string PleigerRemark { get; set; }
    [ColumName("PFE Remark 2")]
    public string? PleigerRemark2 { get; set; }


}

[AttributeUsage(AttributeTargets.All,
                        AllowMultiple = true)]  // Multiuse attribute.  
public class ColumNameAttribute : Attribute
{
    string name;

    public ColumNameAttribute(string name)
    {
        this.name = name;
    }

    public string GetName()
    {
        return name;
    }
}
