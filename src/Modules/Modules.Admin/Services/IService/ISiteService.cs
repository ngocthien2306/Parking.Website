using InfrastructureCore;
using InfrastructureCore.Models.Site;
using System.Collections.Generic;

namespace Modules.Admin.Services.IService
{
    public interface ISiteService
    {
        List<SYSite> GetListData();
        SYSite GetDetail(int siteID);
        SYSite GetDetailByCode(string siteCode);
        Result SaveData(int siteID, string siteCode, string siteName, string siteDescription, string userModify);
        Result UpdateData(SYSite model, string userModify);
        Result DeleteData(int siteID);
    }
}
