using InfrastructureCore;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.Production.Model;
using System;
using System.Collections.Generic;

namespace Modules.Pleiger.Production.Services.IService
{
    public interface IMESProductionService
    {
        List<MES_SaleProject> GetListData(string ProjectStatus, string userProjectCode, string projectName, string prodcnCode, string itemCode, string itemName, 
            string customer, string SalesClassification, string projectOrderType, string saleOrderProjectName, string startDate, string endDate);
        MES_SaleProject GetDetailData(string ProjectCode, string ProjectStatus);
        List<MES_ProductLine> GetListProdLinesMaster();
        List<MES_ProjectProdcnLines> GetListProjectProdcnLines(string projectCode);
        Result SaveProductPlainLines(string ProjectCode, string ProdcnCode, 
            List<MES_ProjectProdcnLines> lstAdd, List<MES_ProjectProdcnLines> lstEdit, 
            List<MES_ProjectProdcnLines> lstDelete, string MaterWHCode, 
            string ProdcnMessage, DateTime PlanDoneDate);
        Result OnUpdateWorkPlan(string ProjectCode, string ProdcnCode, string ProjectStatus, string RequestCode, string MaterialWarehouse, int OrderQty, string UserID);
        Result OnUpdateWorkCompleted(string ProjectCode, string ProdcnCode, string ProjectStatus);
        Result OnUpdateProdLineStatus(string ProjectCode, string ProdcnCode, string ProdcnLineCode, string Status, string ItemCode,
            string FNWarehouse, string RequestCode,string FinishWarehouseCodeSlt, string UserID, string ProductionMessage);
        //Result OnUpdateProdLineDoneQty(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int ProductionQuantity);
        //Result OnUpdateProdLineDoneQtyAndState(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int ProductionQuantity, string Status);
        List<MES_ProductLine> GetProductLine();
        List<MES_SaleProject> GetProjectName(string ProjectStatus);
        List<MES_Item> GetItemName(string ProjectStatus);
        List<MES_SaleProject> GetListDataNew(string ProductLineName, string ProjectName, string itemName);
        Result CheckQtyOfEachItemIsEnoughInWarehouse(string ProjectCode, string ProdcnCode, string RequestCode, 
            string MaterialWarehouse, int OrderQty, string UserID);

        Result OnUpdateProjectReturn(string ProjectCode, string ProdcnCode, string ProjectStatus, string Remark,string ProjectStatusNow);
        List<MES_SaleProject> CheckDataDataWorkMagt(string ProjectCode);
        List<MES_ProjectProdcnLinesStatusChart> GetAllSalesProject();
        List<MES_ProjectProdcnLinesChart> GetAllProjectProdcnLines();
        List<MES_ProjectProdcnWorkResultsChart> GetAllProjectProdcnWorkResults();



        // Huy Code
        Result SaveProductPlainLinesRemake(string ProjectCode, string ProdcnCode,
          List<MES_ProjectProdcnLines> lstAdd, List<MES_ProjectProdcnLines> lstEdit,
          List<MES_ProjectProdcnLines> lstDelete, string MaterWHCode,
          string ProdcnMessage, DateTime PlanDoneDate, int[] PlanRequestQty);
        int SavePlanningFormData(string ProjectCode, string ProdcnCode, string MaterWHCode, string ProdcnMessage, DateTime PlanDoneDate);
        Result SaveLineGridData(string ProjectCode, string ProdcnCode, string Data);
        Result DeleteProductionLine(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int GroupLine);
        #region Work management new
        List<MES_ProjectProdcnLines> getListProductionLine(string projectCode);
        List<MES_ProjectProdcnLines> GetListProductionLinesDataCheckComplete(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int GroupLine, int LineOrder);
        List<ProductLineGroup> getProductionLineGroups(string projectCode);
        ProductLineGroup getProductionLineByGroup(string projectCode, string lineGroup);
        Result OnUpdateProdLineDoneQty(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int ProductionQuantity, string UserID, string UserName, string ItemCode, string WareHouseCode, string MasterWHCode, DateTime? EndDate, DateTime? PlanStartDate, DateTime? PlanEndDate, int CumulativeProductionQuantity, string ProductionMessage,string GroupLine);
        Result OnUpdateProdLineDoneQtyAndState(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int ProductionQuantity, string Status, string UserID, string UserName, string ItemCode, string wareHouseCode, string MasterWHCode, DateTime? EndDate, DateTime? PlanStartDate, DateTime? PlanEndDate, int CumulativeProductionQuantity, string ProductionMessage,string GroupLine);
        Result OnUpdateLastProdLineDoneQty(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int ProductionQuantity, string UserID, string UserName, string ItemCode, string WareHouseCode, string MasterWHCode, DateTime? EndDate, DateTime? PlanStartDate, DateTime? PlanEndDate, int CumulativeProductionQuantity, string ProductionMessage,string GroupLine);
        Result OnUpdateLastProdLineDoneQtyAndState(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int ProductionQuantity, string Status, string UserID, string UserName, string ItemCode, string wareHouseCode, string MasterWHCode, DateTime? EndDate, DateTime? PlanStartDate, DateTime? PlanEndDate, int CumulativeProductionQuantity, string ProductionMessage,string GroupLine);
        int CountGroupLine(string ProdcnCode);
        #endregion
    }
}
