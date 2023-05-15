using InfrastructureCore.DAL;
using InfrastructureCore;
using Modules.Common.Models;
using Modules.Kiosk.Member.Repositories.IRepository;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;

namespace Modules.Kiosk.Member.Repositories.Repository
{
    public class KIOMemberRemoveMgt : IKIOMemberRemoveMgt
    {
        private readonly static string SP_MEMBER_REMOVE_MANAGEMENT = "SP_MEMBER_REMOVE_MANAGEMENT";


        #region Get Data
        public List<KIO_SubscriptionHistory> GetMemberRemove(string storeNo, string userId, int lessMonth, int onceRecently)
        {
            List<KIO_SubscriptionHistory> listMember = new List<KIO_SubscriptionHistory>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[5];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@StoreNo";
                    arrParam[2] = "@UserId";
                    arrParam[3] = "@LessMonth";
                    arrParam[4] = "@OnceRecently";
                    object[] arrValue = new object[5];
                    arrValue[0] = "GetMemberRemove";
                    arrValue[1] = storeNo;
                    arrValue[2] = userId;
                    arrValue[3] = lessMonth;
                    arrValue[4] = onceRecently;
                    listMember = connection.ExecuteQuery<KIO_SubscriptionHistory>(SP_MEMBER_REMOVE_MANAGEMENT, arrParam, arrValue).ToList();
                    return listMember;
                }
            }
            catch
            {
                return listMember;
            }
        }

        public List<KIO_SubscriptionHistory> GetMemberRemoveDetail(string storeNo, string userId)
        {
            List<KIO_SubscriptionHistory> listMember = new List<KIO_SubscriptionHistory>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[3];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@StoreNo";
                    arrParam[2] = "@UserId";
                    object[] arrValue = new object[3];
                    arrValue[0] = "GetMemberRemoveDetail";
                    arrValue[1] = storeNo;
                    arrValue[2] = userId;
                    listMember = connection.ExecuteQuery<KIO_SubscriptionHistory>(SP_MEMBER_REMOVE_MANAGEMENT, arrParam, arrValue).ToList();
                    return listMember;
                }
            }
            catch
            {
                return listMember;
            }
        }

        public List<KIO_UserHistory> GetRemoveHistory(string userId)
        {
            List<KIO_UserHistory> userHistories = new List<KIO_UserHistory>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[2];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@UserId";
                    object[] arrValue = new object[2];
                    arrValue[0] = "GetRemoveHistory";
                    arrValue[1] = userId;
                    userHistories = connection.ExecuteQuery<KIO_UserHistory>(SP_MEMBER_REMOVE_MANAGEMENT, arrParam, arrValue).ToList();
                    return userHistories;
                }
            }
            catch
            {
                return userHistories;
            }
        }
        #endregion

        #region Create - Update - Delete
        public Result DeleteMemberRemove(string storeNo, string userId)
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
                            string[] arrParam = new string[3];
                            arrParam[0] = "@Method";
                            arrParam[1] = "@StoreNo";
                            arrParam[2] = "@UserId";

                            object[] arrValue = new object[3];
                            arrValue[0] = "DeleteMemberRemove";
                            arrValue[1] = storeNo;
                            arrValue[2] = userId;

                            resultDelete = connection.ExecuteScalar<string>(SP_MEMBER_REMOVE_MANAGEMENT, CommandType.StoredProcedure, arrParam, arrValue, transaction);
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
        public Result SaveMemberRemove(SaveUserDto saveUserDto)
        {
            byte[] imgCardPath;
            byte[] imgTakenPath;
            try
            {
                imgCardPath = saveUserDto.imgCardPath == null ? null : System.IO.File.ReadAllBytes(saveUserDto.imgCardPath);
                imgTakenPath = saveUserDto.imgTakenPath == null ? null : System.IO.File.ReadAllBytes(saveUserDto.imgTakenPath);
            }
            catch
            {
                imgCardPath = null;
                imgTakenPath = null;
            }
            var status = (imgCardPath != null && imgTakenPath != null) ? 3 : imgTakenPath != null ? 1 : imgCardPath != null ? 2 : 0;

            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var resultSave = "false";
                            string[] arrParam = new string[9];
                            arrParam[0] = "@Method";
                            arrParam[1] = "@UserId";
                            arrParam[2] = "@UserName";
                            arrParam[3] = "@Birthday";
                            arrParam[4] = "@Gender";
                            arrParam[5] = "@ImgTaken";
                            arrParam[6] = "@ImgCard";
                            arrParam[7] = "@PhoneNumber";
                            arrParam[8] = "@Status";
                            object[] arrValue = new object[9];
                            arrValue[0] = "SaveMemberRemove";
                            arrValue[1] = saveUserDto.userId;
                            arrValue[2] = saveUserDto.userName;
                            arrValue[3] = saveUserDto.birthday;
                            arrValue[4] = saveUserDto.gender;
                            arrValue[5] = imgTakenPath;
                            arrValue[6] = imgCardPath;
                            arrValue[7] = saveUserDto.phone;
                            arrValue[8] = status;
                            resultSave = connection.ExecuteScalar<string>(SP_MEMBER_REMOVE_MANAGEMENT, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if (resultSave != "false")
                            {
                                try
                                {
                                    if (saveUserDto.imgCardPath != null)
                                    {
                                        System.IO.File.Delete(saveUserDto.imgCardPath);

                                    }
                                    if (saveUserDto.imgTakenPath != null)
                                    {
                                        System.IO.File.Delete(saveUserDto.imgTakenPath);
                                    }
                                }
                                catch
                                {

                                }
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
