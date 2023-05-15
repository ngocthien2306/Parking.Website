using InfrastructureCore;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Models.Menu;
using Modules.Admin.Models;
using Modules.Common.Models;

using System.Collections.Generic;

namespace Modules.Admin.Services.IService
{
    public interface IAccessMenuService
    {
        #region "Group Access Menu"

        List<SYGroupAccessMenus> GetListAccessMenuGroupByMenuID(string MenuID, int SiteID);
        List<SYMenu> GetAccessMenuWithUserCode(string UserCode, int SiteID, string UserID);
        List<SYMenu> GetAccessMenuWithSuperAdmin();
        List<SYMenu> GetAccessMenuWithAdmin(int SiteID);
        Result SaveGroupAccessMenu(int MenuID, List<SYUserGroups> data, SYLoggedUser currentUser);

        // Quan add 2020/08/18
        // Get list SYUser Group access Menu By MenuID,UserID
        // Sum col DELETE_FILE_YN 
        // Sum UPLOAD_FILE_SUM      
        List<SYGroupAccessMenus> SelectSumFileUploadByMenuID(int MenuID, string UserID);
        List<SYGroupAccessMenus> SelectUserPermissionInGroup(int MenuID, string UserID);        
        List<SYUserAccessMenus> SelectUserPermissionAccessMenu(int MenuID, string UserID);
        List<SYUserAccessMenus> GetButtonPermissionByUser(int SiteID, int MenuID, string UserCode);


        #endregion

        #region "SYUser Access Menu"

        List<SYUserAccessMenus> GetListAccessMenuUserByMenuID(string MenuID);
        List<SYMenuAccess> GetListAccessToobarWithUser(SYLoggedUser currentUser);
        Result SaveUserAccessMenu(int MenuID, List<SYUserAccessMenus> data, SYLoggedUser currentUser);

        #endregion
    }
}
