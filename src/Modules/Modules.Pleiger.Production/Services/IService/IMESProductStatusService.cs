using DevExtreme.AspNet.Mvc;
using InfrastructureCore;
using Modules.Pleiger.CommonModels;
using System;
using System.Collections.Generic;
using System.Data;

namespace Modules.Pleiger.Production.Services.IService
{
    public interface IMESProductStatusService
    {
        List<MES_ProductStatus> GetProductStatus(DataSourceLoadOptions sourceLoadOptions, MES_ProductStatus productStatus);
        List<MES_ProductStatus> GetDataExportExcelICube(string jsonObj);
        string ExportExcelICube(DataTable dt);

    }
}
