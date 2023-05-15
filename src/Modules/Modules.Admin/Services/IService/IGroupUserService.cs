using InfrastructureCore;
using InfrastructureCore.Models.Identity;
using Modules.Admin.Models;

using System.Collections.Generic;

namespace Modules.Admin.Services.IService
{
    public interface IGroupUserService
    {
        #region "SYUser Group"

        List<SYUserGroups> GetListUserGroup(SYLoggedUser info, int siteId);
        Result SaveDataGroupMaster(SYUserGroups item, SYLoggedUser info, int siteId);
        Result DeleteGroupMaster(SYUserGroups item, SYLoggedUser info, int siteId);

        #endregion

        #region "SYUser in SYUser Group"

        List<SYUsersInGroup> GetListUserGroupSeletedByGroupID(string groupId, int siteId);
        Result SetUserIntoUserGroup(string groupId, List<SYUser> data, SYLoggedUser info, int siteId);
        List<SYUsersInGroup> CheckUserInGroup(string userID, int siteId);

        #endregion
    }
}
