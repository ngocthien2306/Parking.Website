using InfrastructureCore;
using Modules.Admin.Models;
using Modules.Pleiger.CommonModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Production.Services.IService
{
    public interface IMESProductionLineService
    {
        List<MES_ProductLine> searchProductionLines(string InternalExternal, string MaterialWarehouseCode, string ProductionLineNameEng, string ProductionLineNameKor, string ProductionLineCode);
        List<DynamicCombobox> MaterialWarehouseCodeCombobox();
        List<DynamicCombobox> FinishWarehouseCodeCombobox();
        List<DynamicCombobox> ProductManagerLineCombobox();
        List<DynamicCombobox> GetPartnerComboboxCombobox();
        Result CRUDProductLine(List<MES_ProductLine> ArrInsLst, List<MES_ProductLine> ArrUpdLst, List<MES_ProductLine> ArrDelLst, string curUser);
    }
}
