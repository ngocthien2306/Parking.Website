using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Common.Models;
using Modules.Parking.Repositories.IRepo;
using Modules.Pleiger.CommonModels.Models;
using Modules.Pleiger.CommonModels.Parking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Modules.Parking.Repositories.Repo
{
    class VehicleHistoryRepository : IVehicleHistoryRepository
    {
        private readonly static string SP_VEHICLE_HISTORY = "SP_VEHICLE_HISTORY";

        #region Get Data
        public List<ParkingVehicleHistory> GetMemberManagement(string storeNo, string userId, int lessMonth, int onceRecently)
        {
            List<ParkingVehicleHistory> listMember = new List<ParkingVehicleHistory>();
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
                    arrValue[0] = "GetVehicleHistory";
                    arrValue[1] = storeNo;
                    arrValue[2] = userId;
                    arrValue[3] = lessMonth;
                    arrValue[4] = onceRecently;
                    listMember = connection.ExecuteQuery<ParkingVehicleHistory>(SP_VEHICLE_HISTORY, arrParam, arrValue).ToList();
                    return listMember;
                }
            }
            catch
            {
                return listMember;
            }
        }

        public List<VehiceInfo> GetVehiceInfo(string storeNo, string userId, string lp, string vehicleId)
        {
            List<VehiceInfo> listMember = new List<VehiceInfo>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[5];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@StoreNo";
                    arrParam[2] = "@UserId";
                    arrParam[3] = "@plateNum";
                    arrParam[4] = "@VehicleId";
                    object[] arrValue = new object[5];
                    arrValue[0] = "GetVehicleByUser";
                    arrValue[1] = storeNo;
                    arrValue[2] = userId;
                    arrValue[3] = lp;
                    arrValue[4] = vehicleId;
                    listMember = connection.ExecuteQuery<VehiceInfo>(SP_VEHICLE_HISTORY, arrParam, arrValue).ToList();
                    return listMember;
                }
            }
            catch
            {
                return listMember;
            }

        }
        public List<ParkingHistoryDetail> GetMemberManagementDetail(string storeNo, string userId, string lp)
        {
            List<ParkingHistoryDetail> listMember = new List<ParkingHistoryDetail>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[4];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@StoreNo";
                    arrParam[2] = "@UserId";
                    arrParam[3] = "@plateNum";
                    object[] arrValue = new object[4];
                    arrValue[0] = "GetVehicleHistoryDetail";
                    arrValue[1] = storeNo;
                    arrValue[2] = userId;
                    arrValue[3] = lp;
                    listMember = connection.ExecuteQuery<ParkingHistoryDetail>(SP_VEHICLE_HISTORY, arrParam, arrValue).ToList();
                    return listMember;
                }
            }
            catch
            {
                return listMember;
            }
            
        }
        public Result SaveVehicle(VehiceInfo vehice)
        {
            byte[] imgVehicle;
            byte[] imgLicense;
            try
            {
                imgVehicle = vehice.vehiclePhotoPath == null ? null : System.IO.File.ReadAllBytes(vehice.vehiclePhotoPath);
                imgLicense = vehice.licensePhotoPath == null ? null : System.IO.File.ReadAllBytes(vehice.licensePhotoPath);
            }
            catch
            {
                imgVehicle = null;
                imgLicense = null;
            }
            var status = (imgVehicle != null && imgLicense != null) ? 3 : imgLicense != null ? 1 : imgVehicle != null ? 2 : 0;
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var resultSave = "false";
                            string[] arrParam = new string[11];
                            arrParam[0] = "@Method";
                            arrParam[1] = "@UserId";
                            arrParam[2] = "@plateNum";
                            arrParam[3] = "@typeTransport";
                            arrParam[4] = "@typePlate";
                            arrParam[5] = "@vehicleId";
                            arrParam[6] = "@vehiclePhoto";
                            arrParam[7] = "@licensePhoto";
                            arrParam[8] = "@vehiclePhotoPath";
                            arrParam[9] = "@licensePhotoPath";
                            arrParam[10] = "@Status";
                            object[] arrValue = new object[11];
                            arrValue[0] = "SaveVehicle";
                            arrValue[1] = vehice.userId;
                            arrValue[2] = vehice.plateNum;
                            arrValue[3] = vehice.typeTransport;
                            arrValue[4] = vehice.typePlate;
                            arrValue[5] = vehice.id;
                            arrValue[6] = imgVehicle;
                            arrValue[7] = imgLicense;
                            arrValue[8] = vehice.vehiclePhotoPath;
                            arrValue[9] = vehice.licensePhotoPath;
                            arrValue[10] = status;
                            resultSave = connection.ExecuteScalar<string>(SP_VEHICLE_HISTORY, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if (resultSave != "false")
                            {
                                try
                                {
                                    if (vehice.vehiclePhotoPath != null)
                                    {
                                        System.IO.File.Delete(vehice.vehiclePhotoPath);

                                    }
                                    if (vehice.licensePhotoPath != null)
                                    {
                                        System.IO.File.Delete(vehice.licensePhotoPath);
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
                        catch(Exception e)
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

        public Result DeleteVehicle(string vehicleId, string userId)
        {
            throw new NotImplementedException();
        }

        public List<KIO_UserHistory> GetUserHistory(string userId)
        {
            throw new NotImplementedException();
        }
        #endregion
        public Result SaveDataMember(SaveUserDto saveUserDto)
        {
            throw new NotImplementedException();
        }
        public Result DeleteDataMember(string storeNo, string userId)
        {
            throw new NotImplementedException();
        }

    }
}
