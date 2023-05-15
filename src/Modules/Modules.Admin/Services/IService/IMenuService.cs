using InfrastructureCore;
using InfrastructureCore.Models.Menu;
using Modules.Admin.Models;
using Modules.Common.Models;

using System.Collections.Generic;

namespace Modules.Admin.Services.IService
{
    public interface IMenuService
    {
        List<SYMenu> GetListDataByGroup(int siteID);
        List<DropDownInt> GetListMenuLevel();
        List<SYMenu> GetListMenuParent(int siteID, int menuLevel);
        List<DropDownInt> GetListPageLayout();
        List<SYBoardInfo> GetListPageBoard(int siteID);        
        List<DropDownInt> GetListMenuIcon();
        Result InsertUpdateData(int siteID, int menuID, string menuName,string menuNameEng, string menuPath,
            int menuLevel, int? menuParentID, int? menuSeq, string adminLevel, string menuType,
            int? programID, string mobileUse, string intraUse, string menuDesc, string startupPageUse, string isCanClose, int? menuIcon);
        Result DeleteData(int menuID);
    }
}
