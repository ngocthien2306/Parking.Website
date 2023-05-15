using Modules.Pleiger.Models;
using System.Collections.Generic;

namespace Modules.Pleiger.Services.IService
{
    public interface IMESPartnerService
    {
        #region Partner Details
        MES_Partner GetPartnerDetailByPartnerCode(string PartnerCode);
        #endregion
        List<MES_Partner> GetPartnerDetailByPartnerType(string PartnerType);
        List<MES_Partner> GetPartnerDetailByTwoPartnerType(string PartnerType1, string PartnerType2);
        List<MES_Partner> GetPartnerByPartnerCode(string PartnerCode);
        List<MES_Partner> GetAllPartner();
        List<MES_ItemPartner> getListPartner_ByProjectCode(string projectCode);
    }
}
