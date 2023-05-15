using InfrastructureCore;
using InfrastructureCore.DAL;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Models.Menu;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Modules.Admin.Services.ServiceImp
{
    public class AccessMenuService : IAccessMenuService
    {
        private const string SP_Name = "SP_Web_SYGroupAccessMenus";
        private const string SP_Name_User_Access = "SP_Web_SYUserAccessMenus";

        #region "Get Data"

        #region "Group Access Menu"

        // Get list SYUser Group access Menu
        public List<SYGroupAccessMenus> GetListAccessMenuGroupByMenuID(string MenuID, int SiteID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@MenuID";
                arrParams[2] = "@SiteID";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "SelectByMenuID";
                arrParamsValue[1] = MenuID;
                arrParamsValue[2] = SiteID;
                var result = conn.ExecuteQuery<SYGroupAccessMenus>(SP_Name, arrParams, arrParamsValue);
                return result.ToList();
            }
        }

        public List<SYMenu> GetAccessMenuWithUserCode(string UserCode, int SiteID, string UserID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[4];
                arrParams[0] = "@Method";
                arrParams[1] = "@UserCode";
                arrParams[2] = "@SiteID";
                arrParams[3] = "@UserID";
                object[] arrParamsValue = new object[4];
                arrParamsValue[0] = "GetAccessMenuWithUserCode";
                arrParamsValue[1] = UserCode;
                arrParamsValue[2] = SiteID;
                arrParamsValue[3] = UserID;
                var result = conn.ExecuteQuery<SYMenu>(SP_Name, arrParams, arrParamsValue);

                return result.ToList();
            }
        }

        public List<SYMenu> GetAccessMenuWithSuperAdmin()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetAccessMenuWithSuperAdmin";
                var result = conn.ExecuteQuery<SYMenu>(SP_Name, arrParams, arrParamsValue);

                return result.ToList();
            }
        }

        public List<SYMenu> GetAccessMenuWithAdmin(int SiteID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@SiteID";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetAccessMenuWithAdmin";
                arrParamsValue[1] = SiteID;
                var result = conn.ExecuteQuery<SYMenu>(SP_Name, arrParams, arrParamsValue);
                return result.ToList();
            }
        }

        #endregion

        #region "SYUser Access Menu"

        public List<SYUserAccessMenus> GetListAccessMenuUserByMenuID(string MenuID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@MenuID";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "SelectByMenuID";
                arrParamsValue[1] = MenuID;
                var result = conn.ExecuteQuery<SYUserAccessMenus>(SP_Name_User_Access, arrParams, arrParamsValue);

                return result.ToList();
            }
        }

        public List<SYMenuAccess> GetListAccessToobarWithUser(SYLoggedUser currentUser)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@UserID";
                arrParams[2] = "@UserCode";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "GetAccessToobarByUser";
                arrParamsValue[1] = currentUser.UserID;
                arrParamsValue[2] = currentUser.UserCode;
                var result = conn.ExecuteQuery<SYMenuAccess>(SP_Name_User_Access, arrParams, arrParamsValue);

                return result.ToList();
            }
        }

        // Quan add 2020/08/18
        // Get list SYUser Group access Menu By MenuID
        // Sum col DELETE_FILE_YN 
        // Sum UPLOAD_FILE_SUM
        public List<SYGroupAccessMenus> SelectSumFileUploadByMenuID(int MenuID, string UserID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@MenuID";
                arrParams[2] = "@UserID";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "SelectSumDelUploadByMenuID";
                arrParamsValue[1] = MenuID;
                arrParamsValue[2] = UserID;
                var result = conn.ExecuteQuery<SYGroupAccessMenus>(SP_Name, arrParams, arrParamsValue);
                return result.ToList();
            }
        }
        public List<SYGroupAccessMenus> SelectUserPermissionInGroup(int MenuID, string UserID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@MenuID";
                arrParams[2] = "@UserID";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "SelectUserPermissionInGroup";
                arrParamsValue[1] = MenuID;
                arrParamsValue[2] = UserID;
                var result = conn.ExecuteQuery<SYGroupAccessMenus>(SP_Name, arrParams, arrParamsValue);
                return result.ToList();
            }
        }
        public List<SYUserAccessMenus> SelectUserPermissionAccessMenu(int MenuID, string UserID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@MenuID";
                arrParams[2] = "@UserID";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "GetlistpermissionsAccessMenu";
                arrParamsValue[1] = MenuID;
                arrParamsValue[2] = UserID;
                var result = conn.ExecuteQuery<SYUserAccessMenus>(SP_Name, arrParams, arrParamsValue);
                return result.ToList();
            }
        }
        public List<SYUserAccessMenus> GetButtonPermissionByUser(int SiteID, int MenuID, string UserCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[4];
                arrParams[0] = "@Method";
                arrParams[1] = "@SiteID";
                arrParams[2] = "@MenuID";
                arrParams[3] = "@UserCode";

                object[] arrParamsValue = new object[4];
                arrParamsValue[0] = "GetButtonPermissionByUser";
                arrParamsValue[1] = SiteID;
                arrParamsValue[2] = MenuID;
                arrParamsValue[3] = UserCode;
                var result = conn.ExecuteQuery<SYUserAccessMenus>(SP_Name_User_Access, arrParams, arrParamsValue);
                return result.ToList();
            }
        }

        #endregion

        #endregion

        #region "Insert - Update - Delete"

        #region "Group Access Menu"

        public Result SaveGroupAccessMenu(int MenuID, List<SYUserGroups> data, SYLoggedUser currentUser)
        {
            // find all child in parentMenu
                
            //
            Result result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Quan add 2020-11-16
                        string[] arrParamsSelect = new string[3];
                        arrParamsSelect[0] = "@Method";
                        arrParamsSelect[1] = "@MenuID";    
                        arrParamsSelect[2] = "@SiteID";    
                        
                        object[] arrParamsValueSelect = new object[3];
                        arrParamsValueSelect[0] = "SelectListMenuIDByParentID";
                        arrParamsValueSelect[1] = MenuID;                        
                        arrParamsValueSelect[2] = currentUser.SiteID;                        
                        var ListMenuIDByParentID = conn.ExecuteQuery<SYUserAccessMenus>(SP_Name, arrParamsSelect, arrParamsValueSelect,transaction);
                    
                        //string[] arrParams = new string[2];
                        //arrParams[0] = "@Method";
                        //arrParams[1] = "@MenuID";
                        //object[] arrParamsValue = new object[2];
                        //arrParamsValue[0] = "DeleteByMenuID";
                        //arrParamsValue[1] = MenuID;
                        //var rsDel = conn.ExecuteNonQuery(SP_Name, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction);

                        foreach (var itemMenu in ListMenuIDByParentID)
                        {
                            string[] arrParams = new string[2];
                            arrParams[0] = "@Method";
                            arrParams[1] = "@MenuID";
                            object[] arrParamsValue = new object[2];
                            arrParamsValue[0] = "DeleteByMenuID";
                            arrParamsValue[1] = itemMenu.MENU_ID;
                            var rsDel = conn.ExecuteNonQuery(SP_Name, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction);

                            foreach (var item in data)
                            {
                                string[] arrParamsAdd = new string[17];
                                arrParamsAdd[0] = "@Method";
                                arrParamsAdd[1] = "@MenuID";
                                arrParamsAdd[2] = "@GroupID";
                                arrParamsAdd[3] = "@CreateBy";
                                arrParamsAdd[4] = "@SearchYN";
                                arrParamsAdd[5] = "@CreateYN";
                                arrParamsAdd[6] = "@SaveYN";
                                arrParamsAdd[7] = "@DeleteYN";
                                arrParamsAdd[8] = "@EditYN";
                                arrParamsAdd[9] = "@ExcelYN";
                                arrParamsAdd[10] = "@PrintYN";
                                arrParamsAdd[11] = "@DelFileYN";
                                arrParamsAdd[12] = "@UploadFileYN";
                                arrParamsAdd[13] = "@SiteID";
                                //bao add
                                arrParamsAdd[14] = "@INVENTORY_YN ";
                                arrParamsAdd[15] = "@PURCHASE_ORDER_YN";
                                arrParamsAdd[16] = "@EXPORT_EXCEL_ICUBE_YN";

                                object[] arrParamsAddValue = new object[17];
                                arrParamsAddValue[0] = "Insert";
                                //arrParamsAddValue[1] = MenuID;
                                arrParamsAddValue[1] = itemMenu.MENU_ID;
                                arrParamsAddValue[2] = item.GROUP_ID;
                                arrParamsAddValue[3] = currentUser.UserID;
                                arrParamsAddValue[4] = item.SearchYN;
                                arrParamsAddValue[5] = item.CreateYN;
                                arrParamsAddValue[6] = item.SaveYN;
                                arrParamsAddValue[7] = item.DeleteYN;
                                arrParamsAddValue[8] = item.EditYN;
                                arrParamsAddValue[9] = item.ExcelYN;
                                arrParamsAddValue[10] = item.PrintYN;
                                arrParamsAddValue[11] = item.DelFileYN;
                                arrParamsAddValue[12] = item.UploadFileYN;
                                arrParamsAddValue[13] = currentUser.SiteID;
                                arrParamsAddValue[14] = item.INVENTORY_YN;
                                arrParamsAddValue[15] = item.PURCHASE_ORDER_YN;
                                arrParamsAddValue[16] = item.EXPORT_EXCEL_ICUBE_YN;
                                var rsAdd = conn.ExecuteNonQuery(SP_Name, CommandType.StoredProcedure, arrParamsAdd, arrParamsAddValue, transaction);
                            }
                        }
                        transaction.Commit();
                        result.Success = true;
                    }
                    catch 
                    {
                        transaction.Rollback();
                        result.Success = false;
                    }
                }
            }

            return result;
        }

        #endregion

        #region "SYUser Access Menu"

        // Set SYUser Access Menu
        public Result SaveUserAccessMenu(int MenuID, List<SYUserAccessMenus> data, SYLoggedUser currentUser)
        {
            Result result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string[] arrParams = new string[2];
                        arrParams[0] = "@Method";
                        arrParams[1] = "@MenuID";
                        object[] arrParamsValue = new object[2];
                        arrParamsValue[0] = "DeleteByMenuID";
                        arrParamsValue[1] = MenuID;
                        var rsDel = conn.ExecuteNonQuery(SP_Name_User_Access, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction);
                        foreach (var item in data)
                        {
                            string[] arrParamsAdd = new string[13];
                            arrParamsAdd[0] = "@Method";
                            arrParamsAdd[1] = "@MenuID";
                            arrParamsAdd[2] = "@UserID";
                            arrParamsAdd[3] = "@CreateBy";
                            arrParamsAdd[4] = "@SearchYN";
                            arrParamsAdd[5] = "@CreateYN";
                            arrParamsAdd[6] = "@SaveYN";
                            arrParamsAdd[7] = "@DeleteYN";
                            arrParamsAdd[8] = "@EditYN";
                            arrParamsAdd[9] = "@ExcelYN";
                            arrParamsAdd[10] = "@PrintYN";
                            arrParamsAdd[11] = "@DelFileYN";
                            arrParamsAdd[12] = "@UploadFileYN";
                            object[] arrParamsAddValue = new object[13];
                            arrParamsAddValue[0] = "Insert";
                            arrParamsAddValue[1] = MenuID;
                            arrParamsAddValue[2] = item.USER_ID;
                            arrParamsAddValue[3] = currentUser.UserID;
                            arrParamsAddValue[4] = item.SEARCH_YN;
                            arrParamsAddValue[5] = item.CREATE_YN;
                            arrParamsAddValue[6] = item.SAVE_YN;
                            arrParamsAddValue[7] = item.DELETE_YN;
                            arrParamsAddValue[8] = item.EDIT_YN;
                            arrParamsAddValue[9] = item.EXCEL_YN;
                            arrParamsAddValue[10] = item.PRINT_YN;
                            arrParamsAddValue[11] = item.DELETE_FILE_YN;
                            arrParamsAddValue[12] = item.UPLOAD_FILE_YN ;
                            var rsAdd = conn.ExecuteNonQuery(SP_Name_User_Access, CommandType.StoredProcedure, arrParamsAdd, arrParamsAddValue, transaction);
                        }
                        transaction.Commit();
                        result.Success = true;
                    }
                    catch 
                    {
                        transaction.Rollback();
                        result.Success = false;
                    }
                }
            }

            return result;
        }

        #endregion

        #endregion
    }
}
