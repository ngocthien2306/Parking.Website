using DevExpress.XtraReports.Native;
using InfrastructureCore.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace InfrastructureCore.Web.Services.IService
{
    public interface ICommonService
    {
        dynamic ExecuteSvcQuery(DbConnectionEnum dbConnection, string procName, string[] paramNames, object[] paramValues);
        dynamic ExecuteSvcQuery(DbConnectionEnum dbConnection, string procName, string[] paramNames, object[] paramValues, bool useTrans);
        int ExecuteSvcUpdate(DbConnectionEnum dbConnection, DataSet dsChanged, string procName, string[] paramNames);
        int ExecuteSvcUpdate(DbConnectionEnum dbConnection, DataSet dsChanged, string procName, string[] paramNames, bool useTrans);
    }
}
