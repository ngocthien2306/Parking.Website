using Dapper;
using InfrastructureCore;
using InfrastructureCore.Configuration;
using InfrastructureCore.DAL;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Admin.Services.ServiceImp
{
    public class ToolbarService : IToolbarService
    {
        IDBContextConnection dbConnection;
        IDbConnection conn;
        public ToolbarService(IDBContextConnection dbConnection)
        {
            this.dbConnection = dbConnection;
            conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
        }
        private  string SP_Name = "SP_Web_SYDataPageToolbarActionsSPLayout";
        public async Task<List<SYToolbarActions>> GetToolbarActionsWithID(int PageID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {

                string[] arrParams = new string[2];
                arrParams[0] = "@DIV";
                arrParams[1] = "@PAG_ID";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "SelectWithPagID";
                arrParamsValue[1] = PageID;                
                var result = await conn.ExecuteQueryAsync<SYToolbarActions>(SP_Name, arrParams, arrParamsValue).ConfigureAwait(true);

                return result.ToList();
            }
            //var result = new List<SYToolbarActions>();
            //if (conn.State == ConnectionState.Closed)
            //{
            //    conn.Open();
            //}
            //if (conn.State == ConnectionState.Open)
            //{
            //    using (var transaction = conn.BeginTransaction())
            //    {
            //        try
            //        {
            //            var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
            //            dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, "SelectWithPagID");
            //            dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, PageID);
            //            var temp = await SqlMapper.QueryAsync<SYToolbarActions>(conn, SP_Name, param: dyParam, transaction: transaction, commandType: CommandType.StoredProcedure).ConfigureAwait(true);
            //            result = temp.ToList();
            //            transaction.Commit();
            //            conn.Close();
            //        }
            //        catch (Exception ex)
            //        {
            //            conn.Close();
            //            transaction.Rollback();
            //        }
            //    }
            //}
            //return result;
        }
    }
}
