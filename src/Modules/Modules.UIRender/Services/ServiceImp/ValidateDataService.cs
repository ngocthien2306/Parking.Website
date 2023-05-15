using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Pleiger.CommonModels;
using Modules.UIRender.Services.IService;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

namespace Modules.UIRender.Services.ServiceImp
{
    public partial class ValidateDataService : IValidateDataService
    {
        private const string SP_VALIDATE_CRUD_WAREHOUSE_DATA_DYNAMIC = "SP_VALIDATE_CRUD_WAREHOUSE_DATA_DYNAMIC";
        private const string SP_VALIDATE_CRUD_ITEMPARTNER_DATA_DYNAMIC = "SP_VALIDATE_CRUD_ITEMPARTNER_DATA_DYNAMIC";


        #region VALIDATE DATA
        public Result ValidateWarehouseData(List<MES_Warehouse> lstObj)
        {
            Result result = new Result();
            string message = "";
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                //arrParams[0] = "@Method";
                arrParams[0] = "@jsonObj";
                object[] arrParamsValue = new object[1];
                //arrParamsValue[0] = "ValidateWarehouseData";
                arrParamsValue[0] = JsonConvert.SerializeObject(lstObj);

                var data = conn.ExecuteQuery(SP_VALIDATE_CRUD_WAREHOUSE_DATA_DYNAMIC, CommandType.StoredProcedure, arrParams, arrParamsValue);

                if(!string.IsNullOrEmpty(data.Tables[0].Rows[0].ItemArray[0].ToString()))
                {
                    message = data.Tables[0].Rows[0].ItemArray[0].ToString();

                    result.Message = "Warehouse code: " + message + " is dupplicated";
                    result.Success = false;
                }
                else
                {
                    result.Success = true;
                }    
                
            }

            return result;
        }

        public Result ValidateItemPartnerData(List<MES_ItemPartner> lstObj)
        {
            Result result = new Result();
            string message = "";
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                //arrParams[0] = "@Method";
                arrParams[0] = "@jsonObj";
                object[] arrParamsValue = new object[1];
                //arrParamsValue[0] = "ValidateWarehouseData";
                arrParamsValue[0] = JsonConvert.SerializeObject(lstObj);

                var data = conn.ExecuteQuery(SP_VALIDATE_CRUD_ITEMPARTNER_DATA_DYNAMIC, CommandType.StoredProcedure, arrParams, arrParamsValue);

                if (!string.IsNullOrEmpty(data.Tables[0].Rows[0].ItemArray[0].ToString()))
                {
                    message = data.Tables[0].Rows[0].ItemArray[0].ToString();

                    result.Message = "Partner Codes: " + message + " dupplicated";
                    result.Success = false;
                }
                else
                {
                    result.Success = true;
                }
            }

            return result;
        }
        #endregion
    }
}
