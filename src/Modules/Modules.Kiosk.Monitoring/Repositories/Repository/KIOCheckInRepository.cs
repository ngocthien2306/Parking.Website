using DocumentFormat.OpenXml.Wordprocessing;
using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Common.Models;
using Modules.Kiosk.Monitoring.Repositories.IRepository;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Modules.Kiosk.Monitoring.Repositories.Repository
{
    public class KIOCheckInRepository : IKIOCheckInRepository
    {
        private readonly string SP_USER_CHECKIN_MONITORING = "SP_USER_CHECKIN_MONITORING";
        private readonly string SP_STORE_MANAGEMENT = "SP_STORE_MANAGEMENT";
        #region Get Data

        public List<KIO_UserStore> GetUserStoreMgt(string storeNo, string userId, string userType)
        {
            List<KIO_UserStore> listUserStore = new List<KIO_UserStore>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[4];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@StoreNo";
                    arrParam[2] = "@UserId";
                    arrParam[3] = "@UserType";
                    object[] arrValue = new object[4];
                    arrValue[0] = "GetUserStoreMgt";
                    arrValue[1] = storeNo;
                    arrValue[2] = userId;
                    arrValue[3] = userType;
                    listUserStore = connection.ExecuteQuery<KIO_UserStore>(SP_STORE_MANAGEMENT, arrParam, arrValue).ToList();
                    return listUserStore;
                }
            }
            catch
            {
                return listUserStore;
            }
        }
        public List<KIO_CheckInInfo> GetCheckInInfo(string storeNo, string startDate, string endDate, int byMin)
        {
            List<KIO_CheckInInfo> kIO_CheckIns = new List<KIO_CheckInInfo>();

            try
            {
                using(var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[5];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@StartDate";
                    arrParams[2] = "@EndDate";
                    arrParams[3] = "@StoreNo";
                    arrParams[4] = "@ByMin";
                    object[] arrValue = new object[5];
                    arrValue[0] = "GetCheckInInfo";
                    arrValue[1] = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss");
                    arrValue[2] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    arrValue[3] = storeNo;
                    arrValue[4] = byMin;
                    var result = connection.ExecuteQuery<KIO_CheckInInfo>(SP_USER_CHECKIN_MONITORING, arrParams, arrValue);
                    kIO_CheckIns = result.ToList();
                    return kIO_CheckIns; 
                }
            }
            catch
            {
                return kIO_CheckIns;
            }
        }

        public KIO_CheckInInfo GetPhotoById(string userId)
        {
            KIO_CheckInInfo kIO_CheckIns = new KIO_CheckInInfo();

            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[2];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@UserId";
                    object[] arrValue = new object[2];
                    arrValue[0] = "GetPhotoById";
                    arrValue[1] = userId;
                    kIO_CheckIns = connection.ExecuteQuery<KIO_CheckInInfo>(SP_USER_CHECKIN_MONITORING, arrParams, arrValue).SingleOrDefault();
                    return kIO_CheckIns;
                }
            }
            catch
            {
                return kIO_CheckIns;
            }
        }

        #endregion

        #region Create - Update - Delete

        public Result UpdateApproveRejectRemoveUser(string userId, bool status)
        {
            Result result = new Result();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var resultDelete = "false";
                            string[] arrParam = new string[3];
                            arrParam[0] = "@Method";
                            arrParam[1] = "@UserId";
                            arrParam[2] = "@Status";
                            object[] arrValue = new object[3];
                            arrValue[0] = "UpdateApproveRejectRemoveUser";
                            arrValue[1] = userId;
                            arrValue[2] = status;
                            resultDelete = connection.ExecuteScalar<string>(SP_USER_CHECKIN_MONITORING, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if (resultDelete != "false")
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
            catch (Exception e)
            {
                return new Result { Success = false, Message = MessageCode.MD0005 };
            }
        }
        public Result UpdateApproveRejectUser(string userId, bool status)
        {
            Result result = new Result();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var resultDelete = "false";
                            string[] arrParam = new string[3];
                            arrParam[0] = "@Method";
                            arrParam[1] = "@UserId";
                            arrParam[2] = "@Status";
                            object[] arrValue = new object[3];
                            arrValue[0] = "UpdateApproveRejectUser";
                            arrValue[1] = userId;
                            arrValue[2] = status;
                            resultDelete = connection.ExecuteScalar<string>(SP_USER_CHECKIN_MONITORING, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if (resultDelete != "false")
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
            catch (Exception e)
            {
                return new Result { Success = false, Message = MessageCode.MD0005 };
            }
        }
        #endregion
    }
}
