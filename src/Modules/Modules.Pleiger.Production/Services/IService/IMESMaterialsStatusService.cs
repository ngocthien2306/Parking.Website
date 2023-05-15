using Modules.Admin.Models;
using Modules.Pleiger.Production.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Production.Services.IService
{
    public interface IMESMaterialsStatusService
    {
        List<DynamicRadioCheckbox> getTypeRadio(string lang);
        List<DynamicCombobox> getCategoryCombobox(string lang);
        List<MESMaterialsStatus> searchMaterialStatus(string StartDate, string EndDate, string InOutType, string Category, string ItemCode, string ItemName);
    }
}
