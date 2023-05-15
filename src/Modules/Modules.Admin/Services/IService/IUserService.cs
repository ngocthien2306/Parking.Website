using InfrastructureCore;
using InfrastructureCore.Models.Identity;
using Modules.Admin.Models;
using InfrastructureCore.Models.Menu;

using System.Collections.Generic;
using Modules.Common.Models;

namespace Modules.Admin.Services.IService
{
    public interface IUserService
    {
        List<SYUser> GetListDataByCode(string siteCode);
        List<SYUser> GetListData();
        List<SYUser> GetListDataUserBySite(int siteId);
        List<SYUser> GetListDataAll(int siteId);
        List<SYUser> GetListUserAll();
        
        List<SYUser> GetListDataByUserCode(string UserCode);
        Result InsertUpdateData(SYUser dataUser, string userModify);
        Result SaveUser(SYUser dataUser, string userModify);
        Result DeleteData(string userId,string userCode);
        Result ChangePassword(bool adminUpdate, string userID, string oldPassWord, string newPassword,int SiteID);
        Result UpdateUserInformation(string userId, string firstName, string lastName, string email, string oldPassword, string newPassword ,string userTypeName, int SiteID);

        #region SY Group Master

        List<SYUserGroups> GetListGroupBySiteID(int siteID);
        #endregion

        #region SY Group Master For SuperAdmin

        List<SYUserGroups> GetListGroupMasterBySiteID(SYLoggedUser info, int siteID);
        List<SYUser> GetListDataUserBySiteForSuperAdmin(SYLoggedUser info, int siteID);
        List<SYUser> GetListDataUserBySiteSearch(int siteId, SYUser model);
        #endregion
        //add 
        string GetSystemUserTypeByUserId(string userId);

        //PVN Add
        List<SYUserAccessMenus> GetAllUser(int SiteID);
        List<SYMenu> GetAllSite(int SiteID);
        List<SYMenu> GetMenuByUserId(string UserCode, int SiteID);
        List<SYMenu> GetCheckedMenu(string UserCode,int SiteID);
        List<SYGroupAccessMenus> GetPermission(string UserCode, int MenuId, int SiteID);
        List<SYGroupAccessMenus> GetPermissionByGroup(string UserCode, int MenuId, int SiteID,int GroupID);
        List<SYGroupAccessMenus> GetGroupByUser(string UserCode, int MenuId, int SiteID);
        
        Result UpdateUserPermission(List<UserPermissionUpdate> userPermissionUpdates, SYLoggedUser CurrentUser);
        List<SYUsersInGroup> GetUserInGroupsByUserId( string UserId);
        List<SYGroupAccessMenus> GetPermissionByUserAndMenuID(string UserID, int MenuID, int SiteID);

        #region Get User type
        public List<CHECKRESULT> CheckUserRole_Partner(string UserCode);
        public CHECKRESULT CheckUserType(string UserId);
        public CHECKRESULT CheckUserEmployee(string UserCode);
        #endregion
    }
}
