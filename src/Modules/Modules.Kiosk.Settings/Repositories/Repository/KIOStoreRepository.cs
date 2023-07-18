using InfrastructureCore;
using InfrastructureCore.DAL;
using InfrastructureCore.Extensions;
using Modules.Common.Models;
using Modules.Kiosk.Settings.PasswordExtension;
using Modules.Kiosk.Settings.Repositories.IRepository;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Modules.Kiosk.Settings.Repositories.Repository
{
    public class KIOStoreRepository : IKIOStoreRepository
    {
        private readonly string SP_STORE_MANAGEMENT = "SP_STORE_MANAGEMENT";


        #region Get data function

        public List<KIO_StoreMaster> GetStoreMastersByUser(string userId)
        {
            List<KIO_StoreMaster> storeMasterList = new List<KIO_StoreMaster>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[2];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@UserId";
                    object[] arrValue = new object[2];
                    arrValue[0] = "GetStoreMastersByUser";
                    arrValue[1] = userId;
                    storeMasterList = connection.ExecuteQuery<KIO_StoreMaster>(SP_STORE_MANAGEMENT, arrParam, arrValue).ToList();
                    return storeMasterList;
                }
            }
            catch
            {
                return storeMasterList;
            }
        }
        public List<KIO_StoreDevice> GetStoreDevices(string storeNo, string deviceName, string type, string key, string ip, string storeDeviceNo)
        {
            List<KIO_StoreDevice> storeDeviceList = new List<KIO_StoreDevice>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[7];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@StoreNo";
                    arrParam[2] = "@DeviceName";
                    arrParam[3] = "@DeviceType";
                    arrParam[4] = "@KeyNo";
                    arrParam[5] = "@IP";
                    arrParam[6] = "@StoreDeviceNo";

                    object[] arrValue = new object[7];
                    arrValue[0] = "GetStoreDevices";
                    arrValue[1] = storeNo;
                    arrValue[2] = deviceName;
                    arrValue[3] = type;
                    arrValue[4] = key;
                    arrValue[5] = ip;
                    arrValue[6] = storeDeviceNo;
                    storeDeviceList = connection.ExecuteQuery<KIO_StoreDevice>(SP_STORE_MANAGEMENT, arrParam, arrValue).ToList();
                    storeDeviceList.ForEach(s =>
                    {
                        s.deviceKey = HexHelper.DecryptRfc(s.deviceKey);
                    });
                    return storeDeviceList;
                }
            }
            catch(Exception ex)
            {
                return storeDeviceList;
            }
        }

        public List<KIO_HistoryUserStore> GetHistoryManager(string storeNo, string userId)
        {
            List<KIO_HistoryUserStore> histories = new List<KIO_HistoryUserStore>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[3];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@StoreNo";
                    arrParam[2] = "@UserId";
                    object[] arrValue = new object[3];
                    arrValue[0] = "GetHistoryManager";
                    arrValue[1] = storeNo;
                    arrValue[2] = userId;
                    histories = connection.ExecuteQuery<KIO_HistoryUserStore>(SP_STORE_MANAGEMENT, arrParam, arrValue).ToList();
                    return histories;
                }
            }
            catch
            {
                return histories;
            }
        }

        public List<KIO_StoreMaster> GetStoreMasters(string storeNo, string location, string storeName, string siteType)
        {
            List<KIO_StoreMaster> storeMasterList = new List<KIO_StoreMaster>();
            try
            {
                using(var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[5];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@StoreNo";
                    arrParam[2] = "@Location";
                    arrParam[3] = "@StoreName";
                    arrParam[4] = "@SiteType";
                    object[] arrValue = new object[5];
                    arrValue[0] = "GetStoreMasters";
                    arrValue[1] = storeNo;
                    arrValue[2] = location;
                    arrValue[3] = storeName;
                    arrValue[4] = siteType;
                    storeMasterList = connection.ExecuteQuery<KIO_StoreMaster>(SP_STORE_MANAGEMENT, arrParam, arrValue).ToList();
                    return storeMasterList;
                }
            }
            catch
            {
                return storeMasterList;
            }
        }

        public KIO_UserStore GetUserById(string userId)
        {
            KIO_UserStore userStore = new KIO_UserStore();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[2];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@UserId";
                    object[] arrValue = new object[2];
                    arrValue[0] = "GetUserById";
                    arrValue[1] = userId;
                    userStore = connection.ExecuteQuery<KIO_UserStore>(SP_STORE_MANAGEMENT, arrParam, arrValue).SingleOrDefault();
                    return userStore;
                }
            }
            catch
            {
                return userStore;
            }
        }

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

        #endregion

        #region Create - Update - Delete

        public Result ChangesPassword(string password, string userId)
        {
            using(var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using(var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var resultSave = "false";
                        string[] arrParam = new string[3];
                        arrParam[0] = "@Method"; 
                        arrParam[1] = "@Password";
                        arrParam[2] = "@UserId";
                        object[] arrValue = new object[3];
                        arrValue[0] = "ChangesPassword";
                        arrValue[1] = PasswordExtensions.HashPassword(password);
                        arrValue[2] = userId;
                        resultSave = connection.ExecuteScalar<string>(SP_STORE_MANAGEMENT, CommandType.StoredProcedure,arrParam, arrValue, transaction);
                        transaction.Commit();
                        if (resultSave != "false")
                        {
                            return new Result { Success = true, Message = MessageCode.ME0002, Data = password };
                        }
                        else
                        {
                            transaction.Rollback();
                            return new Result { Success = false, Message = MessageCode.ME0003 };
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        return new Result { Success = false, Message = MessageCode.ME0003 };
                    }
                }
            }
        }
        public Result SaveStoreMgt(KIO_UserStore userStore, int siteId, string uid, string pass)
        {
            try
            {
                using(var conneciton = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using(var transaction = conneciton.BeginTransaction())
                    {
                        try
                        {
                            
                            var resultSave = "false";
                            string[] arrParam = new string[11];
                            arrParam[0] = "@Method"; arrParam[1] = "@UserId"; arrParam[2] = "@UserType";
                            arrParam[3] = "@Password"; arrParam[4] = "@UserName"; arrParam[5] = "@PhoneNumber";
                            arrParam[6] = "@Email"; arrParam[7] = "@Birthday"; arrParam[8] = "@StoreNo";
                            arrParam[9] = "@SiteId"; arrParam[10] = "@UID";
            
                            object[] arrValue = new object[11];
                            arrValue[0] = "SaveStoreMgt"; arrValue[1] = userStore.userId; arrValue[2] = userStore.userType;
                            arrValue[3] = userStore.password == null ? PasswordExtensions.HashPassword(pass) : PasswordExtensions.HashPassword(userStore.password); 
                            arrValue[4] = userStore.userName; arrValue[5] = userStore.phoneNumber;
                            arrValue[6] = userStore.email; arrValue[7] = userStore.birthday; arrValue[8] = userStore.storeNo;
                            arrValue[9] = siteId; arrValue[10] = uid;
                            resultSave = conneciton.ExecuteScalar<string>(SP_STORE_MANAGEMENT, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if(resultSave != "false")
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

        public Result SaveStoreMaster(KIO_StoreMaster storeMaster)
        {
            try
            {
                using (var conneciton = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = conneciton.BeginTransaction())
                    {
                        try
                        {
                            var resultSave = "false";
                            string[] arrParam = new string[15];
                            arrParam[0] = "@Method"; arrParam[1] = "@Location"; arrParam[2] = "@StoreName";
                            arrParam[3] = "@BizNumber"; arrParam[4] = "@ZipCode"; arrParam[5] = "@Address1";
                            arrParam[6] = "@Address2"; arrParam[7] = "@BizPhoneNumber"; arrParam[8] = "@OpenDate";
                            arrParam[9] = "@Memo"; arrParam[10] = "@StoreNo";
                            arrParam[11] = "@MonitoringStartime"; arrParam[12] = "@MonitoringEndtime";
                            arrParam[13] = "@Capacity"; arrParam[14] = "@SiteType";
                            object[] arrValue = new object[15];
                            arrValue[0] = "SaveStoreMaster"; arrValue[1] = storeMaster.location; arrValue[2] = storeMaster.storeName;
                            arrValue[3] = storeMaster.bizNumber;
                            arrValue[4] = storeMaster.zipCode; arrValue[5] = storeMaster.address1;
                            arrValue[6] = storeMaster.address2; arrValue[7] = storeMaster.bizPhoneNumber; arrValue[8] = storeMaster.openDate;
                            arrValue[9] = storeMaster.memo; arrValue[10] = storeMaster.storeNo;
                            arrValue[11] = storeMaster.monitoringStartime; arrValue[12] = storeMaster.monitoringEndtime;
                            arrValue[13] = storeMaster.capacity; arrValue[14] = storeMaster.siteType;


                            resultSave = conneciton.ExecuteScalar<string>(SP_STORE_MANAGEMENT, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if (resultSave != "false")
                            {
                                return new Result { Success = true, Message = MessageCode.MD0004, Data = resultSave };
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
