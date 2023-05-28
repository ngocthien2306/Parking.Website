using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Parking.Repositories.IRepo;
using Modules.Pleiger.CommonModels.Parking;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modules.Parking.Repositories.Repo
{
    public class VehicleCheckinRepository : IVehicleCheckinRepository
    {
        private readonly static string VEHICLE_MONITORING = "VEHICLE_MONITORING";
        public List<ParkingCheckin> GetListVehicleCheckin(string startTime, string endTime, string byMin, string storeNo, string status)
        {
            List<ParkingCheckin> checkins = new List<ParkingCheckin>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[6];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@StartTime";
                    arrParam[2] = "@EndTime";
                    arrParam[3] = "@ByMin";
                    arrParam[4] = "@StoreNo";
                    arrParam[5] = "@TrackStatus";

                    object[] arrValue = new object[6];
                    arrValue[0] = "GetListVehicleCheckin";
                    arrValue[1] = startTime;
                    arrValue[2] = endTime;
                    arrValue[3] = byMin;
                    arrValue[4] = storeNo;
                    arrValue[5] = status;

                    checkins = connection.ExecuteQuery<ParkingCheckin>(VEHICLE_MONITORING, arrParam, arrValue).ToList();
                    return checkins;
                }
            }
            catch(Exception ex)
            {
                return checkins;
            }
        }
    }
}
