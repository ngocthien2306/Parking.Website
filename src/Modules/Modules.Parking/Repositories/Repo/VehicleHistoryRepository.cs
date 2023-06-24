using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Parking.Repositories.IRepo;
using Modules.Pleiger.CommonModels.Models;
using Modules.Pleiger.CommonModels.Parking;
using System;
using System.Collections.Generic;
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
