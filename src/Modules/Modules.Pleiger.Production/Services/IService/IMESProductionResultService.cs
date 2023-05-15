using Modules.Pleiger.CommonModels;
using Modules.Pleiger.Production.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Modules.Pleiger.Production.Services.IService
{
    public interface IMESProductionResultService
    {
        public List<MES_SaleProject> searchProductionResult(string userProjectCode, string productionCode, string itemCode, string itemName, string projectStatus, string salesClasification, string projectOrderType, string salesOrderProjectName,string SalesOrderProjectCode);
        public List<MESProductionResultExportTemplate> getExportExcelData(string userProjectCode, string productionCode, string itemCode, string itemName, string projectStatus, string salesClasification, string projectOrderType, string salesOrderProjectName, string SalesOrderProjectCode);
        public List<MES_ProjectProdcnLines> getProductionLineResult(string projectCode, string productionCode);
        public List<MES_ProjectProdcnLines> getProductionLineResultDetail(string projectCode, string GroupLine);

        public List<MESWorkHistory> getWorkHistory(string projectCode, string productionCode, string prodcnLineCode);
        public List<MESProductDelivery> getDeliveryInformation(string projectCode);
        public string ExportItemPartnerExcelFile(DataTable dt);
    }
}
