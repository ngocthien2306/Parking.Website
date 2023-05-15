using InfrastructureCore;
using InfrastructureCore.DAL;
using InfrastructureCore.Extensions;
using Modules.Common.Models;
using Modules.Kiosk.Management.Repositories.IRepositories;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Modules.Kiosk.Management.Repositories.Repositories
{
    public class KIOAccountRepository : IKIOAccountRepository
    {

        private readonly static string SP_ACCOUNT_MANAGEMENT = "SP_ACCOUNT_MANAGEMENT";
        #region Get Data
        public List<KIO_AccountMgt> GetAccountMgt(string userId)
        {
            List<KIO_AccountMgt> accountMgts = new List<KIO_AccountMgt>();
            try
            {
                using(var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[2];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@UserId";
                    object[] arrValue = new object[2];
                    arrValue[0] = "GetAccountMgt";
                    arrValue[1] = userId;
                    accountMgts = connection.ExecuteQuery<KIO_AccountMgt>(SP_ACCOUNT_MANAGEMENT, arrParam, arrValue).ToList();
                    return accountMgts;
                }
            }
            catch
            {
                return accountMgts;
            }
        }

        public List<KIO_UseRegisteredStore> GetUserRegisteredStore(string storeNo, string userId)
        {
            List<KIO_UseRegisteredStore> accountMgts = new List<KIO_UseRegisteredStore>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[3];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@UserId";
                    arrParam[2] = "@StoreNo";
                    object[] arrValue = new object[3];
                    arrValue[0] = "GetUserRegisteredStore";
                    arrValue[1] = userId;
                    arrValue[2] = storeNo;
                    accountMgts = connection.ExecuteQuery<KIO_UseRegisteredStore>(SP_ACCOUNT_MANAGEMENT, arrParam, arrValue).ToList();
                    return accountMgts;
                }
            }
            catch
            {
                return accountMgts;
            }
        }


        #endregion

        #region Create - Update - Delete
        public Result SaveAccountMgt(string userId, bool status)
        {
            using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var resultSave = "false";
                        string[] arrParam = new string[3];
                        arrParam[0] = "@Method";
                        arrParam[1] = "@Status";
                        arrParam[2] = "@UserId";
                        object[] arrValue = new object[3];
                        arrValue[0] = "SaveAccountMgt";
                        arrValue[1] = status;
                        arrValue[2] = userId;
                        resultSave = connection.ExecuteScalar<string>(SP_ACCOUNT_MANAGEMENT, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                        transaction.Commit();
                        if (resultSave != "false")
                        {
                            return new Result { Success = true, Message = MessageCode.MEA001 };
                        }
                        else
                        {
                            transaction.Rollback();
                            return new Result { Success = false, Message = MessageCode.MEA002 };
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        return new Result { Success = false, Message = MessageCode.MEA002 };
                    }
                }
            }
        }
        public Result DeleteUserOutStore(string userId, string storeNo)
        {

            using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var resultSave = "false";
                        string[] arrParam = new string[3];
                        arrParam[0] = "@Method";
                        arrParam[1] = "@StoreNo";
                        arrParam[2] = "@UserId";
                        object[] arrValue = new object[3];
                        arrValue[0] = "DeleteUserOutStore";
                        arrValue[1] = storeNo;
                        arrValue[2] = userId;
                        resultSave = connection.ExecuteScalar<string>(SP_ACCOUNT_MANAGEMENT, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                        transaction.Commit();
                        if (resultSave != "false")
                        {
                            return new Result { Success = true, Message = MessageCode.MD0008 };
                        }
                        else
                        {
                            transaction.Rollback();
                            return new Result { Success = false, Message = MessageCode.MD0015 };
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        return new Result { Success = false, Message = MessageCode.MD0015 };
                    }
                }
            }
        }

        public Result AddUserToStore(string userId, string storeNo)
        {
            using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var resultSave = "false";
                        string[] arrParam = new string[3];
                        arrParam[0] = "@Method";
                        arrParam[1] = "@ListStoreNo";
                        arrParam[2] = "@UserId";
                        object[] arrValue = new object[3];
                        arrValue[0] = "AddUserToStore";
                        arrValue[1] = storeNo;
                        arrValue[2] = userId;
                        resultSave = connection.ExecuteScalar<string>(SP_ACCOUNT_MANAGEMENT, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                        transaction.Commit();
                        if (resultSave != "false")
                        {
                            return new Result { Success = true, Message = MessageCode.MD0004 };
                        }
                        else
                        {
                            transaction.Rollback();
                            return new Result { Success = false, Message = MessageCode.MD0005 };
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        return new Result { Success = false, Message = MessageCode.MD0005 };
                    }
                }
            }
        }
        #endregion
    }
}
