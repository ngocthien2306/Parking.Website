using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Admin.Models;
using Modules.Pleiger.Production.Model;
using Modules.Pleiger.Production.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modules.Pleiger.Production.Services.ServiceImp
{
    class MESMaterialsStatusService : IMESMaterialsStatusService
    {
        private const string SP_GET_RADIO_CHECKBOX_VALUE_INOUT_TYPE = "SP_GET_RADIO_CHECKBOX_VALUE_INOUT_TYPE";
        private const string SP_GET_COMBOBOX_VALUE_CATEGORY = "SP_GET_COMBOBOX_VALUE_CATEGORY";
        private const string SP_INVENTORY_STATUS_GET_DATA = "SP_INVENTORY_STATUS_GET_DATA";

        public List<DynamicCombobox> getCategoryCombobox(string lang)
        {
            var result = new List<DynamicCombobox>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Lang";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = lang;
                var data = conn.ExecuteQuery<DynamicCombobox>(
                    SP_GET_COMBOBOX_VALUE_CATEGORY,
                    arrParams,
                    arrParamsValue);
                result = data.ToList();
            }
            return result;
        }

        public List<MESMaterialsStatus> searchMaterialStatus(string StartDate, string EndDate, string InOutType, string Category, string ItemCode, string ItemName)
        {
            var result = new List<MESMaterialsStatus>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[7];
                arrParams[0] = "@Method";
                arrParams[1] = "@StartDate";
                arrParams[2] = "@EndDate";
                arrParams[3] = "@InOutType";
                arrParams[4] = "@Category";
                arrParams[5] = "@ItemCode";
                arrParams[6] = "@ItemName";
                object[] arrParamsValue = new string[7];
                arrParamsValue[0] = "SearchInventoryStatus";
                arrParamsValue[1] = StartDate;
                arrParamsValue[2] = EndDate;
                arrParamsValue[3] = InOutType;
                arrParamsValue[4] = Category;
                arrParamsValue[5] = ItemCode;
                arrParamsValue[6] = ItemName;
                var data = conn.ExecuteQuery<MESMaterialsStatus>(
                    SP_INVENTORY_STATUS_GET_DATA, arrParams, arrParamsValue);
                result = data.ToList();
            }
            int i = 1;
            result.ForEach(x => x.No = i++);
            return result;
        }

        public List<DynamicRadioCheckbox> getTypeRadio(string lang)
        {
            var result = new List<DynamicRadioCheckbox>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Lang";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = lang;
                var data = conn.ExecuteQuery<DynamicRadioCheckbox>(
                    SP_GET_RADIO_CHECKBOX_VALUE_INOUT_TYPE,
                    arrParams,
                    arrParamsValue);
                result = data.ToList();
            }
            return result;
        }
    }
}
