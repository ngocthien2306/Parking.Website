using InfrastructureCore;
using Modules.Pleiger.CommonModels;
using System.Collections.Generic;

namespace Modules.UIRender.Services.IService
{
    public partial interface IValidateDataService
    {
        Result ValidateWarehouseData(List<MES_Warehouse> lstObj);

        Result ValidateItemPartnerData(List<MES_ItemPartner> lstObj);
    }
}
