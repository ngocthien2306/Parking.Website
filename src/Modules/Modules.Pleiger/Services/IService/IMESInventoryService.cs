using InfrastructureCore;
using Microsoft.AspNetCore.Http;
using Modules.Pleiger.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Modules.Pleiger.Services.IService
{
    public interface IMESInventoryService
    {
        #region "Trans Closing Mst"
        List<MES_TransClosingMst> GetTransClosingMst(string startDate, string endDate);
        #endregion
        #region "Trans Closing Detail"
        List<MES_TransClosingDtls> GetTransClosingDtl(string TransMonth);
        #endregion

        #region "duy viet Trans Closing Detail TransCloseNo"
        List<MES_TransClosingDtls> GetTransClosingDtlTransCloseNo(string TransCloseNo);
        #endregion
        #region "duy viet Trans Closing Master TransMonth"
        List<MES_TransClosingMst> GetTransClosingMstTransMonth(string TransMonth);
        #endregion
        #region "Trans Closing items"
        List<MES_TransClosingItems> GetTransClosingItems(string TransCloseNo);
        List<MES_TransClosingItems> GetTransClosingMstSearch(string startDate, string endDate, string Category, string ItemClass, string ItemCode, string ItemName);
        #endregion

        #region Inventory check
        bool IsTransmonthHaveInventoryClosed(string data);
        List<MES_InventoryCheckVO> GetInventoryCurrentStock(string warehouseCode, string category,
            string itemCode, string itemName, DateTime? closeMonth);

        List<MES_InventoryCheckVO> GetInventoryCurrentStockNew(string warehouseCode, string category,
            string itemCode, string itemName, DateTime? closeMonth, string Lang);

        List<MES_InventoryCheckExcelVO> GetItemInventoryByWHCodeAndItemCode(string jsonObj);
        List<MES_InventoryCheckVO> GetInventoryCurrentStockDownload(string warehouseCode, string category, string itemCode, string itemName);
        string ExportInventoryExcelFile(DataTable dt);

        Result UploadFileInventoryCurrentStock(IFormFile myFile, string chunkMetadata, Type type, string Lang);
        Result SaveInventoryCheck(List<MES_InventoryCheckVO> data, string userModify, string detailRemark);
        Result CloseMonth(MES_TransClosingMst data, string userModify);
        Result UnCloseMonth(List<MES_TransClosingMst> data, string userModify);

        List<MES_Warehouse> GetWareHouseByCategory(string CategoryCode);
        #endregion
    }
}
