using InfrastructureCore;
using Modules.Pleiger.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Services.IService
{
    public interface IMESProductionService
    {
        List<MES_SaleProject> GetListData(string ProjectStatus, string projectCode, string prodcnCode, string itemCode, string itemName, string customer);
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
            string FNWarehouse, string RequestCode,string FinishWarehouseCodeSlt, string UserID);
        //Result OnUpdateProdLineDoneQty(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int ProductionQuantity);
        //Result OnUpdateProdLineDoneQtyAndState(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int ProductionQuantity, string Status);
        Result OnUpdateProdLineDoneQty(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int ProductionQuantity, string UserID, string UserName, string ItemCode, string WareHouseCode, string MasterWHCode);
        Result OnUpdateProdLineDoneQtyAndState(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int ProductionQuantity, string Status, string UserID, string UserName, string ItemCode, string wareHouseCode, string MasterWHCode);

        List<MES_ProductLine> GetProductLine();
        List<MES_SaleProject> GetProjectName(string ProjectStatus);
        List<MES_Item> GetItemName(string ProjectStatus);
        List<MES_SaleProject> GetListDataNew(string ProductLineName, string ProjectName, string itemName);
        Result CheckQtyOfEachItemIsEnoughInWarehouse(string ProjectCode, string ProdcnCode, string RequestCode, 
            string MaterialWarehouse, int OrderQty, string UserID);

    }
}
