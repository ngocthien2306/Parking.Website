using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Admin.Models;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.Production.Model;
using Modules.Pleiger.Production.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modules.Pleiger.Production.Services.ServiceImp
{
    public class MESProjectWarehouseInventoryService : IMESProjectWarehouseInventoryService
    {
        private const string SP_MES_PROJECT_WAREHOUSE_INVENTORY = "SP_MES_PROJECT_WAREHOUSE_INVENTORY";
        private const string SP_MES_GET_PROJECT_WAREHOUSE_INVENTORY = "SP_MES_GET_PROJECT_WAREHOUSE_INVENTORY";
        private const string SP_MES_GET_COMBOBOX_WAREHOUSE_INVENTORY = "SP_MES_GET_COMBOBOX_WAREHOUSE_INVENTORY";


        public List<MES_ComWareHouseInventory> GetComBoBoxWareHouseInventory()
        {
            try
            {
                var result = new List<MES_ComWareHouseInventory>();
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    var data = conn.ExecuteQuery<MES_ComWareHouseInventory>(SP_MES_GET_COMBOBOX_WAREHOUSE_INVENTORY, null, null);
                    result = data.ToList();
                    
                }
             
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<MESProjectWarehouseInventory> SearchProjectWarehouseInventory(string warehouseName, string productionProjectCode, string category)
        {
            var result = new List<MESProjectWarehouseInventory>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[4];
                arrParams[0] = "@Method";
                arrParams[1] = "@WarehouseName";
                arrParams[2] = "@ProductionProjectCode";
                arrParams[3] = "@Category";
                object[] arrParamsValue = new string[4];
                arrParamsValue[0] = "search";
                arrParamsValue[1] = warehouseName;
                arrParamsValue[2] = productionProjectCode;
                arrParamsValue[3] = category;
                var data = conn.ExecuteQuery<MESProjectWarehouseInventory>(
                    SP_MES_PROJECT_WAREHOUSE_INVENTORY, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }

        public List<MESProjectWarehouseInventory> SearchProjectWarehouseInventoryNew(string warehouseName, string productionProjectCode, string category,string SalesOrderProjectCode)
        {
            var result = new List<MESProjectWarehouseInventory>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[6];
                arrParams[0] = "@ProjectCode";
                arrParams[1] = "@WarehouseName";
                arrParams[2] = "@ProductionProjectCode";
                arrParams[3] = "@Category";
                arrParams[4] = "@Method";
                arrParams[5] = "@SalesOrderProjectCode";
                object[] arrParamsValue = new string[6];
                arrParamsValue[0] = null;
                arrParamsValue[1] = warehouseName;
                arrParamsValue[2] = productionProjectCode;
                arrParamsValue[3] = category;
                arrParamsValue[4] = "SearchProjectWarehouseInventoryNew";
                arrParamsValue[5] = SalesOrderProjectCode;
                
                result = conn.ExecuteQuery<MESProjectWarehouseInventory>(
                    SP_MES_GET_PROJECT_WAREHOUSE_INVENTORY, arrParams, arrParamsValue).ToList();
                
            }
            int i = 1;
            result.ForEach(x => x.No = i++);
            return result;

        }

        public List<MESProjectWarehouseInventory> GetProjectWarehouseInventoryDetail(string ProjectCode, string WarehouseCode)
        {
            var result = new List<MESProjectWarehouseInventory>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                arrParams[2] = "@WarehouseCode";
                object[] arrParamsValue = new string[3];
                arrParamsValue[0] = "GetProjectWarehouseInventoryDetail";
                arrParamsValue[1] = ProjectCode;
                arrParamsValue[2] = WarehouseCode;
                result = conn.ExecuteQuery<MESProjectWarehouseInventory>(
                    SP_MES_GET_PROJECT_WAREHOUSE_INVENTORY, arrParams, arrParamsValue).ToList();

            }
            int i = 1;
            result.ForEach(x => x.No = i++);
            return result;

        }

        public List<MESProjectWarehouseInventory> GetProjectWarehouseInventoryListDetail(string listWarehouseInventory)
        {
            var result = new List<MESProjectWarehouseInventory>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@listWarehouseInventory";
 
                object[] arrParamsValue = new string[2];
                arrParamsValue[0] = "GetProjectWarehouseInventoryListDetail";
                arrParamsValue[1] = listWarehouseInventory;

                result = conn.ExecuteQuery<MESProjectWarehouseInventory>(
                    SP_MES_GET_PROJECT_WAREHOUSE_INVENTORY, arrParams, arrParamsValue).ToList();

            }
            int i = 1;
            result.ForEach(x => x.No = i++);
            return result;
        }


    }

}

