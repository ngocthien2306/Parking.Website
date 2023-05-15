using InfrastructureCore;
using Modules.Admin.Models;
using Modules.Pleiger.CommonModels;
using System;
using System.Collections.Generic;
using System.Text;


namespace Modules.Pleiger.SalesProject.Services.IService
{
    public interface IMESSaleOrderProjectService
    {
        List<DynamicCombobox> GetProjectOrderType(string group);
        Result SaveDataSaleProject(MES_SalesOrderProjectNew model, string id);
        Result DeleteSaleOrderProject(string id);
        List<MES_SalesOrderProjectNew> GetListProjectOrder(MES_SalesOrderProjectNew model);
        MES_SalesOrderProjectNew GetDetailSaleOrderProject(string projectCode);
        MES_SalesOrderProjectNew GetDetailSaleOrderProjectNew(string projectCode);
        
        int ValidateName(string name);
    } 
}
