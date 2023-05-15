using InfrastructureCore;
using Modules.Admin.Models;
using Modules.Pleiger.CommonModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.SalesProject.Services.IService
{
    public interface IMESProductionProjectService
    {
        List<MES_SalesOrderProjectNew> GetListSalesOrderProjectPopup(string ProjectOrderType, string OrderNumber, string SalesOrderProjectName,string SalesOrderProjectCode);

        List<MES_SaleProject> SearchRequestProduction(MES_SaleProject model);


        MES_SaleProject GetListSalesProjectDetail(string ProjectCode);

        List<DynamicCombobox> GetAllCustomerCombobox();

        List<DynamicCombobox> GetAllOrderTeamCombobox();

        Result SaveRequestProduction(List<MES_SaleProject> list,string userID,string ProdReqRequestType);

        Result SaveSingleRequestProduction(string ProjectCode,string userID);

    }
}
