using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Common.Models;
using Modules.Kiosk.Settings.PasswordExtension;
using Modules.Kiosk.Settings.Repositories.IRepository;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Modules.Kiosk.Settings.Repositories.Repository
{
    public class KIOEquipmentRepository : IKIOEquipmentRepository
    {
        private readonly string SP_EQUIQMENT_SETTINGS = "SP_EQUIQMENT_SETTINGS";
        public Result DeleteStoreDevice(string storeNo)
        {
            Result result = new Result();
            try
            {
                using(var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1)) {
                    using(var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var resultDelete = "false";
                            string[] arrParam = new string[2];
                            arrParam[0] = "@Method";
                            arrParam[1] = "@StoreDeviceNo";
                            object[] arrValue = new object[2];
                            arrValue[0] = "DeleteStoreDevice";
                            arrValue[1] = storeNo;
                            resultDelete = connection.ExecuteScalar<string>(SP_EQUIQMENT_SETTINGS, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if(resultDelete != "false")
                            {
                                return new Result { Success = true, Message = MessageCode.MESTD4 };
                            }
                            else
                            {
                                transaction.Rollback();
                                return new Result { Success = false, Message = MessageCode.MESTD3 };

                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            return new Result { Success = false, Message = MessageCode.MESTD3 };

                        }
                    }
                }
            }
            catch(Exception e)
            {
                return new Result { Success = false, Message = MessageCode.MESTD3 };
            }
        }

        public Result SaveDataStoreDevice(KIO_StoreDevice storeDevice, string userId)
        {
            Result result = new Result();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using(var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var resultSave = "false";
                            string[] arrParam = new string[15];
                            arrParam[0] = "@Method";
                            arrParam[1] = "@StoreDeviceNo";
                            arrParam[2] = "@DeviceName";
                            arrParam[3] = "@DeviceType";
                            arrParam[4] = "@DeviceKeyNo";
                            arrParam[5] = "@DevicePublicIP";
                            arrParam[6] = "@DeviceUsePort";
                            arrParam[7] = "@DeviceStatus";
                            arrParam[8] = "@RDPPath";
                            arrParam[9] = "@UserId";
                            arrParam[10] = "@StoreNo";
                            arrParam[11] = "@RegistDate";
                            arrParam[12] = "@DeviceKey";
                            arrParam[13] = "@Network";
                            arrParam[14] = "@Threshold";

                            object[] arrValue = new object[15];
                            arrValue[0] = "SaveDataStoreDevice";
                            arrValue[1] = storeDevice.storeDeviceNo;
                            arrValue[2] = storeDevice.deviceName;
                            arrValue[3] = storeDevice.deviceType;
                            arrValue[4] = storeDevice.deviceKeyNo;
                            arrValue[5] = storeDevice.devicePublicIp;
                            arrValue[6] = storeDevice.deviceUsePort;
                            arrValue[7] = storeDevice.deviceStatus;
                            arrValue[8] = storeDevice.rdpPath;
                            arrValue[9] = userId;
                            arrValue[10] = storeDevice.storeNo;
                            arrValue[11] = storeDevice.registDate;
                            arrValue[12] = HexHelper.EncryptRfc(storeDevice.deviceKey);
                            arrValue[13] = storeDevice.network;
                            arrValue[14] = storeDevice.threshold;

                            resultSave = connection.ExecuteScalar<string>(SP_EQUIQMENT_SETTINGS, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if (resultSave != "false")
                            {
                                return new Result { Success = true, Message = MessageCode.MESTS2 };
                            }
                            else
                            {
                                transaction.Rollback();
                                return new Result { Success = false, Message = MessageCode.MESTS1 };
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            return new Result { Success = false, Message = MessageCode.MESTS1 };
                        }
                    }
                }
            }
            catch
            {
                return new Result { Success = false, Message = MessageCode.MESTS1 };
            }
        }

        public Result UpdateStatusDevice(string storeNo, string storeDeviceNo, bool status)
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
                            var resultUpdate = "false";
                            string[] arrParam = new string[4];
                            arrParam[0] = "@Method";
                            arrParam[1] = "@StoreNo";
                            arrParam[2] = "@StoreDeviceNo";
                            arrParam[3] = "@DeviceStatus";
                            object[] arrValue = new object[4];
                            arrValue[0] = "UpdateStatusDevice";
                            arrValue[1] = storeNo;
                            arrValue[2] = storeDeviceNo;
                            arrValue[3] = status;
                            resultUpdate = connection.ExecuteScalar<string>(SP_EQUIQMENT_SETTINGS, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if (resultUpdate != "false")
                            {
                                return new Result { Success = true, Message = MessageCode.MESTD5 };
                            }
                            else
                            {
                                transaction.Rollback();
                                return new Result { Success = false, Message = MessageCode.MESTD6 };

                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            return new Result { Success = false, Message = MessageCode.MESTD6 };

                        }
                    }
                }
            }
            catch (Exception e)
            {
                return new Result { Success = false, Message = MessageCode.MESTD3 };
            }
        }
    }
}
