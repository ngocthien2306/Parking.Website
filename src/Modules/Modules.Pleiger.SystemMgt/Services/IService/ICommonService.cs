using System;
using System.Collections.Generic;
using System.Text;
using Modules.Pleiger.CommonModels; 

namespace Modules.Pleiger.SystemMgt.Services.IService
{
    public interface ICommonService
    {
        List<MES_ComCodeDtls> GetAllComCodeDTL(string lang);
        List<MES_Item> GetAllItem(string lang);
        List<MES_Item> GetItemRaw(string lang);
    }
}
