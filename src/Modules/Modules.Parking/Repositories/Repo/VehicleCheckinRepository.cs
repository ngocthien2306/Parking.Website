using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Common.Models;
using Modules.Parking.Repositories.IRepo;
using Modules.Pleiger.CommonModels.Parking;
using System;
using System.Collections.Generic;
using System.Data;
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
            catch (Exception ex)
            {
                return checkins;
            }
        }

        public Result UpdateApproveRejectVehicle(string plateNum, bool status, int trackId)
        {
            Result result = new Result();
            try
            {
                string vehicleStatus = status ? "VEHI03" : "VEHI01";
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var resultDelete = "false";
                            string[] arrParam = new string[4];
                            arrParam[0] = "@Method";
                            arrParam[1] = "@PlateNum";
                            arrParam[2] = "@StatusVehicle";
                            arrParam[3] = "@TrackNo";
                            object[] arrValue = new object[4];
                            arrValue[0] = "UpdateApproveRejectVehicle";
                            arrValue[1] = plateNum;
                            arrValue[2] = vehicleStatus;
                            arrValue[3] = trackId;
                            resultDelete = connection.ExecuteScalar<string>(VEHICLE_MONITORING, CommandType.StoredProcedure, arrParam, arrValue, transaction);
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
    }

}

