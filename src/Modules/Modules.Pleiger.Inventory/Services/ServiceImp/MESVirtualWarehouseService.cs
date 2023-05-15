using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.Inventory.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modules.Pleiger.Inventory.Services.ServiceImp
{
    public class MESVirtualWarehouseService : IMESVirtualWarehouseService
    {
        private const string SP_MES_VIRTUALWAREHOUSE_CRUD = "SP_MES_VIRTUALWAREHOUSE_CRUD";
        private const string SP_MES_VIRTUALWAREHOUSE_SEARCH = "SP_MES_VIRTUALWAREHOUSE_SEARCH";

        public List<MES_VirtualWarehouse> GetListAllData()
        {
            var result = new List<MES_VirtualWarehouse>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetListAllData";
                var data = conn.ExecuteQuery<MES_VirtualWarehouse>(SP_MES_VIRTUALWAREHOUSE_CRUD, arrParams, arrParamsValue);

                result = data.ToList();
            }



            return result;
        }

        public List<MES_VirtualWarehouse> SearchVirtualWarehouse(MES_VirtualWarehouse model)
        {
            var result = new List<MES_VirtualWarehouse>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[6];
                arrParams[0] = "@Method";
                arrParams[1] = "@VirtualWareHouseId";
                arrParams[2] = "@VirtualWareHouseName";
                arrParams[3] = "@ItemCode";
                arrParams[4] = "@CreateDate";
                arrParams[5] = "@Status";


                object[] arrParamsValue = new object[6];
                arrParamsValue[0] = "SearchVirtualWarehouse";
                arrParamsValue[1] = model.VirtualWareHouseId;
                arrParamsValue[2] = model.VirtualWareHouseName;
                arrParamsValue[3] = model.ItemCode;
                arrParamsValue[4] = model.CreateDate;
                arrParamsValue[5] = model.Status;

                var data = conn.ExecuteQuery<MES_VirtualWarehouse>(SP_MES_VIRTUALWAREHOUSE_SEARCH, arrParams, arrParamsValue);

                result = data.ToList();
            }

            //int i = 1;
            //result.ForEach(x => x.No = i++);

            return result;
        }

        public Result SaveVirtualWarehouse(MES_VirtualWarehouse model, string userModify, string type)
        {
            using var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1);
            var result = -1;
            try
            {
                //sai cho64 nay/////////////// client truyen tham so sai
                result = conn.ExecuteNonQuery(SP_MES_VIRTUALWAREHOUSE_CRUD,
                    new string[] { "@Method",
                                   "@VirtualWareHouseId","@VirtualWareHouseName",
                                   "@ItemCode", "@ItemQty","@Description", "@CreateDate", "@Creater",
                                   "@Status", "@CloseDate"
                    },
                    new object[] { type,
                                    model.VirtualWareHouseId, model.VirtualWareHouseName,
                                    model.ItemCode, model.ItemQty, model.Description,
                                    model.CreateDate, model.Creater,
                                    model.Status, model.CloseDate
                                 });
                if (result > 0)
                {
                    return new Result
                    {
                        Success = true,
                        Message = "Save data success!"
                    };
                }
                else
                {
                    return new Result
                    {
                        Success = false,
                        Message = "Save data not success!",
                    };
                }
            }
            catch (Exception ex)
            {
                return new Result
                {
                    Success = false,
                    Message = "Save data not success! + Exception: " + ex.ToString(),
                };
            }
        }

        public MES_VirtualWarehouse GetDataDetail(string Id)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {

                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@VirtualWareHouseId";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetDetail";
                arrParamsValue[1] = Id;
                var result = conn.ExecuteQuery<MES_VirtualWarehouse>(SP_MES_VIRTUALWAREHOUSE_CRUD, arrParams, arrParamsValue);

                return result.FirstOrDefault();
            }
        }
        public MES_VirtualWarehouse GetVirtualWareHousesDetailByID(string Id)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {

                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@VirtualWareHouseId";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetDetailFileID";
                arrParamsValue[1] = Id;
                var result = conn.ExecuteQuery<MES_VirtualWarehouse>(SP_MES_VIRTUALWAREHOUSE_CRUD, arrParams, arrParamsValue);

                return result.FirstOrDefault();
            }
        }
        
        public Result DeleteVirtualWarehouse(string[] VirtualWareHouseId)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = -1;
                foreach (var id in VirtualWareHouseId)
                {
                    //check project status 01 moi dc delete .ko thi quang loi ra
                    //itemQuery = GetDataDetail(id);
                    //if (!itemQuery.ProjectStatus.Equals("PJST01"))
                    //{
                    //    return new Result
                    //    {
                    //        Success = false,
                    //        Message = "Delete data not success!",
                    //        Data = ""
                    //    };
                    //}
                    result = conn.ExecuteNonQuery(SP_MES_VIRTUALWAREHOUSE_CRUD,
                                       new string[] { "@Method", "@VirtualWareHouseId" },
                                       new object[] { "Delete", id });
                }
                if (result > 0)
                {
                    return new Result
                    {
                        Success = true,
                        Message = "Delete data success!"
                    };
                }
                else
                {
                    return new Result
                    {
                        Success = false,
                        Message = "Delete data not success !",
                        Data = VirtualWareHouseId
                    };
                }
            }
        }
    }
}

