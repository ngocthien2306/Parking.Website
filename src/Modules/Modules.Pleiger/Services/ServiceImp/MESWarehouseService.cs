﻿using Modules.Pleiger.Services.IService;
using InfrastructureCore;
using Modules.Pleiger.Models;
using InfrastructureCore.DAL;
using System.Linq;
using System.Collections.Generic;
using Modules.Admin.Models;

namespace Modules.Pleiger.Services.ServiceImp
{
    public class MESWarehouseService : IMESWarehouseService
    {
        private const string SP_MES_WAREHOUSE = "SP_MES_WAREHOUSE";

        public List<DynamicCombobox> GetAllPartnerWarehouse(string PartnerCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<DynamicCombobox>(SP_MES_WAREHOUSE,
                    new string[] { "@DIV", "@PartnerCode" },
                    new object[] { "PartnerWH", PartnerCode }).ToList();
                return result;
            }
        }

        public List<DynamicCombobox> GetAllPleigerWarehouse()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<DynamicCombobox>(SP_MES_WAREHOUSE,
                    new string[] { "@DIV"},
                    new object[] { "PleigerWH" }).ToList();
                return result;
            }
        }
        public List<DynamicCombobox> GetAllPleigerMaterialWarehouse()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<DynamicCombobox>(SP_MES_WAREHOUSE,
                    new string[] { "@DIV"},
                    new object[] { "PleigerWHMaterial" }).ToList();
                return result;
            }
        }
        public List<DynamicCombobox> GetAllPleigerFinishProductWarehouse()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<DynamicCombobox>(SP_MES_WAREHOUSE,
                    new string[] { "@DIV"},
                    new object[] { "PleigerWHFNProduct" }).ToList();
                return result;
            }
        }

        #region "Validate"
        public bool WarehouseValidateDuplicate(string WarehouseCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Warehouse>(SP_MES_WAREHOUSE,
                    new string[] { "@DIV", "@WarehouseCode" },
                    new object[] { "ValidateWarehouseExisted", WarehouseCode }).ToList().FirstOrDefault();
                return result != null ? false : true;
            }
        }

        #endregion
        #region Get List WH by Type
        public List<MES_Warehouse> GetListWareHouseByType(string WareHouseType)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Warehouse>(SP_MES_WAREHOUSE,
                    new string[] { "@DIV", "@WarehouseType" },
                    new object[] { "GetListByType", WareHouseType }).ToList();
                return result;
            }
        }
        #endregion

    }
}