using DocumentFormat.OpenXml.Spreadsheet;
using InfrastructureCore;
using InfrastructureCore.DAL;
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
    public class KIOCommonCodeRepository : IKIOCommonCodeRepository
    {
        private readonly string SP_COMMON_CODE = "SP_COMMON_CODE";

        #region Get Data
        public List<KIO_CommonCode> GetCommonCode(string code, string subCode, bool status)
        {
            List<KIO_CommonCode> listCode = new List<KIO_CommonCode>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[4];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@Code";
                    arrParam[2] = "@SubCode";
                    arrParam[3] = "@Status";
                    object[] arrValue = new object[4];
                    arrValue[0] = "GetCommonCode";
                    arrValue[1] = code;
                    arrValue[2] = subCode;
                    arrValue[3] = status;
                    listCode = connection.ExecuteQuery<KIO_CommonCode>(SP_COMMON_CODE, arrParam, arrValue).ToList();
                    return listCode; 
                }
            }
            catch(Exception e)
            {
                return listCode;
            }
        }
        public List<KIO_MasterCode> GetMasterCode(string code, bool status)
        {
            List<KIO_MasterCode> listCode = new List<KIO_MasterCode>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[3];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@Code";
                    arrParam[2] = "@Status";
                    object[] arrValue = new object[3];
                    arrValue[0] = "GetMasterCode";
                    arrValue[1] = code;
                    arrValue[2] = status;
                    listCode = connection.ExecuteQuery<KIO_MasterCode>(SP_COMMON_CODE, arrParam, arrValue).ToList();
                    return listCode;
                }
            }
            catch
            {
                return listCode;
            }
        }
        #endregion

        #region Create - Update - Delete
        public Result DeleteCommonCode(string code)
        {
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var resultDelete = "false";
                            string[] arrParam = new string[2];
                            arrParam[0] = "@Method";
                            arrParam[1] = "@SubCode";

                            object[] arrValue = new object[2];
                            arrValue[0] = "DeleteCommonCode";
                            arrValue[1] = code;
                     
                            resultDelete = connection.ExecuteScalar<string>(SP_COMMON_CODE, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if (resultDelete != "false")
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
            catch
            {
                return new Result { Success = false, Message = MessageCode.MD0015 };
            }
        }
        public Result DeleteMasterCode(string code)
        {
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var resultDelete = "false";
                            string[] arrParam = new string[2];
                            arrParam[0] = "@Method";
                            arrParam[1] = "@Code";

                            object[] arrValue = new object[2];
                            arrValue[0] = "DeleteMasterCode";
                            arrValue[1] = code;

                            resultDelete = connection.ExecuteScalar<string>(SP_COMMON_CODE, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if (resultDelete != "false")
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
            catch
            {
                return new Result { Success = false, Message = MessageCode.MD0015 };
            }
        }
        public Result SaveCommonCode(KIO_CommonCode commonCode)
        {
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var resultSave = "false";
                            string[] arrParam = new string[7];
                            arrParam[0] = "@Method";
                            arrParam[1] = "@SubCode";
                            arrParam[2] = "@Code";
                            arrParam[3] = "@Name1";
                            arrParam[4] = "@Name2";
                            arrParam[5] = "@Description";
                            arrParam[6] = "@SystemCode";
                            object[] arrValue = new object[7];
                            arrValue[0] = "SaveCommonCode";
                            arrValue[1] = commonCode.commonSubCode;
                            arrValue[2] = commonCode.commonCode;
                            arrValue[3] = commonCode.commonSubName1;
                            arrValue[4] = commonCode.commonSubName2;
                            arrValue[5] = commonCode.description;
                            arrValue[6] = commonCode.systemCode;

                            resultSave = connection.ExecuteScalar<string>(SP_COMMON_CODE, CommandType.StoredProcedure, arrParam, arrValue, transaction);
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
            catch
            {
                return new Result { Success = false, Message = MessageCode.MD0005 };
            }
        }
        public Result SaveMasterCode(KIO_MasterCode masterCode)
        {
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var resultSave = "false";
                            string[] arrParam = new string[6];
                            arrParam[0] = "@Method";
                            arrParam[1] = "@Code";
                            arrParam[2] = "@Name1";
                            arrParam[3] = "@Name2";
                            arrParam[4] = "@Description";
                            arrParam[5] = "@SystemCode";
                            object[] arrValue = new object[6];
                            arrValue[0] = "SaveMasterCode";
                            arrValue[1] = masterCode.commonCode;
                            arrValue[2] = masterCode.commonName1;
                            arrValue[3] = masterCode.commonName2;
                            arrValue[4] = masterCode.description;
                            arrValue[5] = masterCode.systemCode;

                            resultSave = connection.ExecuteScalar<string>(SP_COMMON_CODE, CommandType.StoredProcedure, arrParam, arrValue, transaction);
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
            catch
            {
                return new Result { Success = false, Message = MessageCode.MD0005 };
            }
        }
        #endregion
    }
}
