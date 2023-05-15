using Modules.Pleiger.Models;
using System;
using System.Collections.Generic;
using System.Text;
using InfrastructureCore.Models.Menu;

namespace Modules.Pleiger.Services.IService
{
    public interface ICommonService
    {
        List<MES_ComCodeDtls> GetAllComCodeDTL(string lang);
        List<MES_Item> GetAllItem(string lang);
        List<MES_Item> GetItemRaw(string lang);

    }
}
