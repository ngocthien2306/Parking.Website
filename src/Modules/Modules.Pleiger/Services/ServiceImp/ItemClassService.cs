using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modules.Pleiger.Services.ServiceImp
{
    public class ItemClassService : IItemClassService
    {
        private string SP_Name = "SP_MES_ITEMCLASS";

        #region "Get Data"

        // Get list Item Class
        public List<MES_ItemClass> GetListData()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetAllData";
                var result = conn.ExecuteQuery<MES_ItemClass>(SP_Name, arrParams, arrParamsValue);

                return result.ToList();
            }
        }

        // Get list Item Up Code
        public List<MES_ItemClass> GetListItemUpCode(string itemComCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@ItemComCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListItemUpCode";
                arrParamsValue[1] = itemComCode;
                var result = conn.ExecuteQuery<MES_ItemClass>(SP_Name, arrParams, arrParamsValue);

                return result.ToList();
            }
        }
        // Get list Item ByCategory
        public List<MES_ItemClass> GetItemClassByCategory()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetItemClassByCategory";
                var result = conn.ExecuteQuery<MES_ItemClass>(SP_Name, arrParams, arrParamsValue);

                return result.ToList();
            }
        }

        // Get list Item ByCategory
        public List<MES_ItemClass> GetItemClassByCategory0304()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetItemClassByCategory0304";
                var result = conn.ExecuteQuery<MES_ItemClass>(SP_Name, arrParams, arrParamsValue);

                return result.ToList();
            }
        }

        //Get All Item Class Code Base On MES_ItemClass table WHERE Category IN ('IMTP01','IMTP02')
        public List<MES_ItemClass> GetItemClassCodeByCategory()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetItemCodeClassByCategory";
                var result = conn.ExecuteQuery<MES_ItemClass>(SP_Name, arrParams, arrParamsValue);

                return result.ToList();
            }
        }
        // Get Item Class by code
        public MES_ItemClass GetItemClassByCode(string itemClassCode, string itemComCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@ItemClassCode";
                arrParams[2] = "@ItemComCode";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "GetItemClassByCode";
                arrParamsValue[1] = itemClassCode;
                arrParamsValue[2] = itemComCode;
                var result = conn.ExecuteQuery<MES_ItemClass>(SP_Name, arrParams, arrParamsValue);

                return result.FirstOrDefault();
            }
        }

        #endregion

        #region "Insert - Update - Delete"

        // Save item Class
        public Result SaveItemClass(string itemClassCode, string itemComCode, string itemUpCode, string itemCategory, string classNameKor, string classNameEng, string etc, string userModify)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = -1;
                try
                {
                    // set default item itemUpCode parent = 00
                    string upcode = itemUpCode != null ? itemUpCode : "0";

                    result = conn.ExecuteNonQuery(SP_Name,
                        new string[] { "@Method", "@ItemClassCode", "@ItemComCode", "@ItemUpCode", "@Category", "@ClassNameKor", "@ClassNameEng", "@Etc", "@UserModify" },
                        new object[] { "SaveData", itemClassCode.Trim(), itemComCode.Trim(), upcode.Trim(), itemCategory.Trim(),
                            classNameKor.Trim(), classNameEng.Trim(), etc, userModify });
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
        }

        // Delete Item Class
        public Result DeleteItemClass(string itemClassCode, string itemComCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery(SP_Name,
                        new string[] { "@Method", "@ItemClassCode", "@ItemComCode" },
                        new object[] { "DeleteData", itemClassCode, itemComCode });
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
        }

        #endregion



        #region Get data shown in combo box-search form
        public List<MES_ItemClass> GetListItemClassByCategory(string categorySelected)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@Category";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListItemClassByCategory";
                arrParamsValue[1] = categorySelected;
                var result = conn.ExecuteQuery<MES_ItemClass>(SP_Name, arrParams, arrParamsValue);

                return result.ToList();
            }
        }
        public List<MES_ItemClass> GetListItemClassSub1ByItemClass(string categorySelected, string itemClassSelected)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@Category";
                arrParams[2] = "@ItemUpCode";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "GetListItemClassSub1ByItemClass";
                arrParamsValue[1] = categorySelected;
                arrParamsValue[2] = itemClassSelected;
                var result = conn.ExecuteQuery<MES_ItemClass>(SP_Name, arrParams, arrParamsValue);

                return result.ToList();
            }
        }
        public List<MES_ItemClass> GetListItemClassSub2ByItemClassSub1(string categorySelected, string itemClassSub1Selected)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@Category";
                arrParams[2] = "@ItemUpCode";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "GetListItemClassSub2ByItemClassSub1";
                arrParamsValue[1] = categorySelected;
                arrParamsValue[2] = itemClassSub1Selected;
                var result = conn.ExecuteQuery<MES_ItemClass>(SP_Name, arrParams, arrParamsValue);

                return result.ToList();
            }
        }
        #endregion
    }
}
