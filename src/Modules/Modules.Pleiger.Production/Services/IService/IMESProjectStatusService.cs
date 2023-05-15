using Modules.Admin.Models;
using Modules.Pleiger.CommonModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Production.Services.IService
{
    public interface IMESProjectStatusService
    {
        List<DynamicCombobox> getProductTypeCombobox(string itemClassCD, string lang);
        List<MES_SaleProject> getMESSaleProject(); 
        List<MES_SaleProject> searchMESSaleProject(string ProjectOrderType, string SaleOrderProjectCode, string projectStatus, string productType, string projectName, string userProjectCode, string SalesClassification,string Userlogin,string checkCode);
        List<MES_SaleProject> SearchSalesStatusByCustomer(string ProjectOrderType, string SaleOrderProjectName, string ProductionProjectCode, string ProductProjectName, string ProductProjectStatus, string Customer, string LoginUser);
        List<Combobox> GetCustomer(string lang);
        

    }
}
